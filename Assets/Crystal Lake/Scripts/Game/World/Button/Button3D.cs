using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MyProj
{
    public class Button3D : MonoBehaviour, IInteractible
    {
        [SerializeField] private ScriptableButton3D preset;
        [SerializeField] private Direction pressDirection = Direction.Backward;
        [SerializeField] private Collider btnColider;
        [SerializeField, Tooltip("Одноразовая")] private bool IsDisposable = false;
        [SerializeField, Tooltip("Только нажать")] private bool onlyPress = false;

        public UnityEvent OnPressed;
        public UnityEvent OnReleased;

        private Vector3 basePosition;
        private Transform thisTransform;
        private Button3DAnimator animator;
        private bool isPressed = false;
        private bool isLocked = false;

        private void Start()
        {
            thisTransform = transform;
            basePosition = thisTransform.localPosition;
            animator = new Button3DAnimator(preset);
        }

        private void OnDisable()
        {
            animator?.Dispose();
        }

        public void Interact(RaycastHit hit, AllPartPlayer allPartPlayer)
        {
            if (isPressed || isLocked)
                return;

            if (onlyPress)
                InteractOnlyPressAsync(hit).Forget();
            else
                InteractAsync(hit).Forget();
        }

        private async UniTask InteractAsync(RaycastHit hit)
        {
            try
            {
                LockButton();

                isPressed = true;
                await animator.AnimatePress(thisTransform, pressDirection);
                OnPressed?.Invoke();
                await animator.AnimateRelease(thisTransform, basePosition);
                OnReleased?.Invoke();
            }
            finally 
            {
                isPressed = false;
            }
        }

        private async UniTask InteractOnlyPressAsync(RaycastHit hit)
        {
            try
            {
                LockButton();

                isPressed = true;
                await animator.AnimatePress(thisTransform, pressDirection);
                OnPressed?.Invoke();
            }
            finally
            {
                isPressed = false;
            }
        }

        private void LockButton()
        {
            if (IsDisposable || onlyPress)
            {
                isLocked = true;
                btnColider.enabled = false;
            }
        }
    }
}
