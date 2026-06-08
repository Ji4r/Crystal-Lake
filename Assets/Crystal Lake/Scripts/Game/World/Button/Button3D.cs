using Cysharp.Threading.Tasks;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;
using TriInspector;

namespace MyProj
{
    [DrawWithTriInspector]
    public class Button3D : NetworkBehaviour, IInteractible
    {
        [SerializeField] private ScriptableButton3D preset;

        private static object FixMaterial()
        {
            throw new NotImplementedException();
        }

        [SerializeField] private Direction pressDirection = Direction.Backward;
        [SerializeField] private Collider btnCollider;
        [SerializeField, Tooltip("Одноразовая кнопка")]
        private bool isDisposable = false;
        [SerializeField, Tooltip("Остаётся нажатой навсегда")]
        private bool onlyPress = false;

        public UnityEvent OnPressed;
        public UnityEvent OnReleased;

        private Vector3 basePosition;
        private Transform cachedTransform;
        private Button3DAnimator animator;

        [SyncVar(hook = nameof(OnPressedStateChanged))]
        private bool isPressed;

        [SyncVar]
        private bool isLocked;

        private void Start()
        {
            cachedTransform = transform;
            basePosition = cachedTransform.localPosition;
            animator = new Button3DAnimator(preset);
        }

        private void OnDisable()
        {
            animator?.Dispose();
        }

        public void Interact(RaycastHit hit, AllPartPlayer allPartPlayer)
        {
            CmdPressButton();
        }

        [Command(requiresAuthority = false)]
        private void CmdPressButton()
        {
            if (isPressed || isLocked)
                return;

            isPressed = true;

            if (isDisposable || onlyPress)
            {
                isLocked = true;

                if (btnCollider != null)
                    btnCollider.enabled = false;
            }

            // Игровая логика выполняется только на сервере
            OnPressed?.Invoke();

            if (!onlyPress)
            {
                ReleaseRoutine().Forget();
            }
        }

        [Server]
        private async UniTaskVoid ReleaseRoutine()
        {
            await UniTask.Delay(
                TimeSpan.FromSeconds(preset.PressDepth));

            isPressed = false;

            OnReleased?.Invoke();
        }

        private void OnPressedStateChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                AnimatePress().Forget();
            }
            else
            {
                AnimateRelease().Forget();
            }
        }

        private async UniTask AnimatePress()
        {
            await animator.AnimatePress(cachedTransform,pressDirection);
        }

        private async UniTask AnimateRelease()
        {
            await animator.AnimateRelease(
                cachedTransform,
                basePosition);
        }
    }
}
