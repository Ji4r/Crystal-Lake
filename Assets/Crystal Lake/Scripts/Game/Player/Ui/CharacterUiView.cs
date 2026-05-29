using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MyProj
{
    public class CharacterUiView : MonoBehaviour, ILocalOnly, IExitHandler
    {
        [SerializeField] private GameObject canvas;
        [SerializeField] private AllPartPlayer allPartPlayer;
        [Header("StaminaBar")]
        [SerializeField] private StaminaController staminaController;
        [SerializeField] private Image leftStaminaImg;
        [SerializeField] private Image rightStaminaImg;
        [SerializeField] private float durationHideStaminaBar = 1f;

        [Header("Window")]
        [SerializeField] private GameObject voiceVolume;
        [SerializeField] private GameObject staminaLevel;
        [SerializeField] private GameObject batteryPercentage;

        [Header("Ui Elements")]
        [SerializeField] private TextMeshProUGUI batteryPercentageText;
        [SerializeField] private Image batteryfieldImage;

        [Header("Notes")]
        [SerializeField] private GameObject notesWindow;
        [SerializeField] private TextMeshProUGUI notesText;

        private Flashlight flashlight;
        private CharacterManager characterManager;
        private CharacterUiView characterUiView;
        private MouseLook mouseLook;
        private IInputReader inputReader;

        private void OnEnable()
        {
            flashlight = allPartPlayer.Get<Flashlight>();
            flashlight.ChangeBatteryLevel += ChangeBatteryLevel;
            staminaController.StaminaChanged += OnStaminaChanged;
        }

        private void OnDisable()
        {
            staminaController.StaminaChanged -= OnStaminaChanged;
            flashlight.ChangeBatteryLevel -= ChangeBatteryLevel;
        }

        private void Start()
        {
            voiceVolume.SetActive(DifficultyGame.Instance.Current.ShowVoiceVolume);
            staminaLevel.SetActive(DifficultyGame.Instance.Current.ShowStaminaLevel);
            batteryPercentage.SetActive(DifficultyGame.Instance.Current.ShowBatteryPercentage);

            characterManager = allPartPlayer.Get<CharacterManager>();
            mouseLook = allPartPlayer.Get<MouseLook>();
            characterUiView = allPartPlayer.Get<CharacterUiView>();
            inputReader = characterManager.GetComponent<IInputReader>();
        }

        private void ChangeBatteryLevel(float max, float current)
        {
            batteryPercentageText.text = $"{current}%";
            batteryfieldImage.fillAmount = current / max;
        }

        private void OnStaminaChanged(float maxStamina, float currentStamina)
        {
            leftStaminaImg.fillAmount = currentStamina / maxStamina;
            rightStaminaImg.fillAmount = currentStamina / maxStamina;
        }

        public void LocalDissable()
        {
            canvas.SetActive(false);
            this.enabled = false;
        }

        public void ShowNotes(string text)
        {
            notesText.text = text;
            notesWindow.SetActive(true);

            inputReader.SetActiveMap(MapInputSystem.UI);
            mouseLook.SetStateCursor(true);
            characterManager.SetExitHandler(this);
        }

        public void HideNotes()
        {
            notesWindow.SetActive(false);
            inputReader.SetActiveMap(MapInputSystem.GAMEPLAY);
            mouseLook.SetStateCursor(false);
            characterManager.SetExitHandler(null);
        }

        public void UpdateTextNotes(string text)
        {
            notesText.text = text;
        }

        public void Exit()
        {
            HideNotes();
        }
    }
}
