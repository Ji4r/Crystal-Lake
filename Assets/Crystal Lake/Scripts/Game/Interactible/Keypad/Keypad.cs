using Mirror;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace MyProj
{
    public class Keypad : NetworkBehaviour, IInteractible, IExitHandler
    {
        [SerializeField] private int keyPassword;
        [SerializeField, Tooltip("Максимальное количество попыток открыть замок без сирены")]
        private int maxNumberOfAttemptsOpenLock;
        [SerializeField] private BoxCollider colider;
        [SerializeField] private Camera cameraKeypad;
        [SerializeField] private TextMeshPro DisplayText;
        [SerializeField, Tooltip("Длительность появление текста")] private float durationAppearanceText;
        [SerializeField, Tooltip("Текст когда пароль верный")] private string textIsOpenDoor = "Open";

        [Header("Ссылка на дверь")]
        [SerializeField, Tooltip("На объекте должен быть реализован интерфейс IUseKeypadCode")] 
        private MonoBehaviour lockObject;

        [Header("Ссылки на записки")]
        [SerializeField] private SpawnerByPoints spawnerByPoints;
        [SerializeField] private NotesWithCode notesWithCodePrefab;
        private NotesWithCode[] notesWithCodes;

        [SyncVar] private bool isPropertyAvailable; // Свободен объект для взаимодействия
        [SyncVar] private bool keypadIsOpen;
        [SyncVar] private byte numberOfAttemptsOpenLock;

        private IUseKeypadCode objectWithKeypad;
        private MouseLook mouseLook;
        private CharacterManager characterManager;
        private IInputReader inputReader;
        private string userInput;

        private void Start()
        {
            objectWithKeypad = lockObject as IUseKeypadCode;

            if (objectWithKeypad == null)
            {
                Debug.LogError("Объект не реализует интерфейс IUseKeypadCode");
            }

            #region SpawnNotesWithCode

            // Сюда нужно внедрить зависимость от уровня сложности

            notesWithCodes = new NotesWithCode[2];
            var points = spawnerByPoints.GetFreeRandomPos();
            notesWithCodes[0] = Instantiate(notesWithCodePrefab, points.position, points.rotation);
            notesWithCodes[0].transform.SetParent(points);
            notesWithCodes[0].SetCode(keyPassword.ToString().Substring(0, 2), 0, (byte)keyPassword.ToString().Length);

            points = spawnerByPoints.GetFreeRandomPos();
            notesWithCodes[1] = Instantiate(notesWithCodePrefab, points.position, points.rotation);
            notesWithCodes[1].transform.SetParent(points);
            notesWithCodes[1].SetCode(keyPassword.ToString().Substring(2), 2, (byte)keyPassword.ToString().Length);
            #endregion
        }

        public override void OnStartServer()
        {
            isPropertyAvailable = true;
            keypadIsOpen = false;
            numberOfAttemptsOpenLock = 0;
        }

        private void OnDisable()
        {
            if (cameraKeypad != null && cameraKeypad.gameObject.activeSelf)
                ExitKeypad();
        }

        public void AddValue(string value)
        {
            if (value == "Enter")
            {
                CheckPassword();
                return;
            }
            userInput += value;
            DisplayText.text = userInput;
        }

        [ClientRpc]
        private void RpcPlaySiren()
        {
            // Тут должен быть код для которая активируеться если игрок превысил количество попыток открыть замок, например включение сирены
        }

        private void CheckPassword()
        {
            if (ParseCode())
            {
                objectWithKeypad?.OpenLock();
                CmdSetStateKeypad(true);
                StartCoroutine(AnimationTextOpen());
                userInput = string.Empty;
                CmdSetNumberOfAttemptsOpenLock(0);
            }
            else
            {
                CmdAddAttempt();
                userInput = string.Empty;
                DisplayText.text = String.Empty;
            }
        }

        private bool ParseCode()
        {
            if (int.TryParse(userInput, out var resault))
            {
               return resault == keyPassword;
            }

            return false;
        }

        private IEnumerator AnimationTextOpen()
        {
            float timeSimbol =  durationAppearanceText / (float)textIsOpenDoor.Length;
            DisplayText.text = String.Empty;

            foreach (var c in textIsOpenDoor)
            {
                yield return new WaitForSeconds(timeSimbol);
                DisplayText.text += c;
            }

            yield return new WaitForSeconds(0.1f);
            ExitKeypad();
        }

        public void Interact(RaycastHit hit, AllPartPlayer allPartPlayer)
        {
            if (!isPropertyAvailable)
                return;

            if (!keypadIsOpen)
            {
                CmdSetAvailable(false);
                this.mouseLook = allPartPlayer.Get<MouseLook>();
                this.characterManager = allPartPlayer.Get<CharacterManager>();
                inputReader = characterManager.GetComponent<IInputReader>();

                characterManager.SetExitHandler(this);
                cameraKeypad.gameObject.SetActive(true);
                mouseLook.SetStateCursor(true);
                colider.enabled = false;
                inputReader.SetActiveMap(MapInputSystem.UI);
                //characterMnagement.cameraControll.controllerUi.SetActiveUi(!characterMnagement.cameraControll.controllerUi.ActiveUi); // кароче тут включается ui
            }
        }

        public void Exit()
        {
            ExitKeypad();
        }

        private void ExitKeypad()
        {
            if (keypadIsOpen)
                colider.enabled = false;
            else
                colider.enabled = true;

            cameraKeypad.gameObject.SetActive(false);
            mouseLook.SetStateCursor(false);
            inputReader.SetActiveMap(MapInputSystem.GAMEPLAY);
            CmdSetAvailable(true);
            characterManager.ClearExitHandler(this);
        }


        [Command(requiresAuthority = false)]
        private void CmdSetAvailable(bool value)
        {
            isPropertyAvailable = value;
        }

        [Command(requiresAuthority = false)]
        private void CmdSetStateKeypad(bool value)
        {
            keypadIsOpen = value;
        }

        [Command(requiresAuthority = false)]
        private void CmdSetNumberOfAttemptsOpenLock(byte value)
        {
            numberOfAttemptsOpenLock = value;
        }

        [Command(requiresAuthority = false)]
        private void CmdAddAttempt()
        {
            numberOfAttemptsOpenLock++;

            if (numberOfAttemptsOpenLock >= maxNumberOfAttemptsOpenLock)
            {
                RpcPlaySiren();
                numberOfAttemptsOpenLock = 0;
            }
        }
    }
}