using DG.Tweening;
using Mirror;
using UnityEngine;

namespace MyProj
{
    public enum DoorDirection { OnYourself, FromMyself, BothWays } // На себя, от себя, в обе стороны

    public class Door : NetworkBehaviour, IInteractible
    {
        [Header("Право созидателя")]
        [SerializeField]
        private bool editorOpen;

        [Header("Обычные настройки")]
        [SyncVar(hook = nameof(SetState))]
        public bool isOpen = false;

        public float durationAnimsDoor = 0.5f;
        public float openAngle = 90;
        public DoorDirection directionOpen;
        public Vector3 ratationDoor;
        [HideInInspector] public Transform transformDoor;
        [SerializeField, Tooltip("Ссылка на замок")] DoorLock doorLock;

        private void Awake()
        {
            transformDoor = GetComponent<Transform>();
            ratationDoor = transformDoor.localRotation.eulerAngles;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            isOpen = editorOpen;
        }
#endif

        public void Interact(RaycastHit hit, AllPartPlayer allPartPlayer)
        {
            allPartPlayer.Get<CharacterInteracter>().CmdToggleDoor(netIdentity);
        }

        public void SetState(bool oldValue, bool newValue)
        {
            if (!isOpen)
            {
                ClosedDoor();
            }
            else
            {
                OpenDoor();
            }
        }

        private void ClosedDoor()
        {
            transformDoor.DORotate(ratationDoor, durationAnimsDoor).SetEase(Ease.OutBack);
        }

        private void OpenDoor()
        {
            Vector3 targetRotation = ratationDoor;

            if (directionOpen == DoorDirection.BothWays)
            {
                Vector3 playerSite = Camera.main.transform.position;
                float calculation = (transform.position.x - playerSite.x) +
                                    (transform.position.z - playerSite.z); // Я хз как это работает но оно работтает "Цтыта Иван"

                float openAngle = (calculation < 0) ? this.openAngle : -this.openAngle;
                targetRotation.y += openAngle;
            }
            else if (directionOpen == DoorDirection.OnYourself)
            {
                targetRotation.y += openAngle;
            }
            else if (directionOpen == DoorDirection.FromMyself)
            {
                targetRotation.y -= openAngle;
            }
            //SoundManager.PlaySound(Sound.CrackDoor);
            transformDoor.DORotate(targetRotation, durationAnimsDoor);
        }
    }
}
