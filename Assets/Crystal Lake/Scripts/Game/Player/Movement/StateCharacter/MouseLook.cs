using UnityEngine;
using Mirror;

namespace MyProj
{
    public class MouseLook : NetworkBehaviour, IControllableMouse, ILocalOnly
    {
        [SerializeField] private float mouseSensivity = 0.1f;
        [SerializeField] private Transform playerBody;
        [SerializeField] private Camera cameraPlayer;
        [SerializeField] private AudioListener audioListener;

        [Header("Углы осмотра")]
        [SerializeField] private float minXRotatin = -90f;
        [SerializeField] private float maxXRotatin = 90f;

        public bool CursorIsHide { get; private set; }

        private Transform cameraTrans;
        private float xRotation = 0f;

        void Awake()
        {
            cameraTrans = cameraPlayer.transform;
            SetHideCursor(false);
        }

        public override void OnStartClient()
        {
            if (!isLocalPlayer)
            {
                LocalDissable();
            }
        }

        public void Look(Vector2 direction)
        {

            float mouseX = direction.x * mouseSensivity;
            float mouseY = direction.y * mouseSensivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minXRotatin, maxXRotatin);

            cameraTrans.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            playerBody.Rotate(Vector3.up * mouseX);
        }

        public void SetHideCursor(bool onEnabled)
        {
            CursorIsHide = onEnabled;

            Cursor.visible = onEnabled;
            Cursor.lockState = onEnabled ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void LocalDissable()
        {
            audioListener.enabled = false;
            cameraPlayer.gameObject.SetActive(false);
            this.enabled = false;
        }
    }
}
