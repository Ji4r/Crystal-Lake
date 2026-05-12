using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace MyProj
{
    [RequireComponent(typeof(CharacterController), typeof(StaminaController))]
    public class CharacterMovement : NetworkBehaviour, ILocalOnly
    {
        [Header("Право созидателя")]
        [SerializeField] private float editorSetCurrent;

        [Header("Обычные настройки")]
        [SerializeField] private AllPartPlayer allPartPlayer;

        [SerializeField] private float knockbackDamping = 5f;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float sprintSpeed;
        [SerializeField] private float crounchSpeed;
        [SerializeField] private float durationTransitionSpeed;
        [SerializeField, Tooltip("Высота колайдера при приседании")] 
        private float heightOnCrounch;

        public event Func<TypeOfActivity, bool> ChangeStaminaAction;
        public event Action OnStartWalking;
        public event Action OnStartSprinting;
        public event Action OnStartCrouching;
        public event Action OnStopMoving;
        public event Action OnStopSprinting;

        public bool IsSprint { get => isSprint;}
        public bool IsCrounch { get => isCrounch;}
        public bool IsMoving { get => isMoving;}

        [SyncVar, SerializeField] private float speedCurrent;
        [SerializeField] private Transform cameraCharacter;

        private float baseHeight;
        private Coroutine switchSpeedCoroutine;
        private CharacterController controller;
        private Vector3 direction;
        private bool isSprint;
        private bool isCrounch;
        private bool isMoving;
        private Vector3 externalVelocity;
        private float knockbackTimer;
        private CharacterAnimator animator;
        private float animatorForward;
        private Vector3 baseCenter;
        private Vector3 baseCameraPosition;

        private void Awake()
        {
            animator = allPartPlayer.Get<CharacterAnimator>();
            controller = GetComponent<CharacterController>();
            baseHeight = controller.height;
            baseCameraPosition = cameraCharacter.localPosition;
            baseCenter = controller.center;

            isSprint = false;
            isCrounch = false;
            isMoving = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            speedCurrent = editorSetCurrent;
        }
#endif

        public void ApplyKnockback(Vector3 force, float duration = 0.3f)
        {
            externalVelocity = force;
            knockbackTimer = duration;
        }

        private void Start()
        {
            speedCurrent = walkSpeed;
            OnStopMoving?.Invoke();
        }

        public void Move(Vector3 direction)
        {
            Vector2 clearInput = new Vector2(direction.x, direction.z);
            this.direction = direction;
            direction = transform.right * direction.x + transform.forward * direction.z;
            // 👉 если есть откидывание — можно отключить управление
            if (knockbackTimer > 0)
            {
                controller.Move(externalVelocity * Time.deltaTime);

                externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, knockbackDamping * Time.deltaTime);
                knockbackTimer -= Time.deltaTime;

                return;
            }

            if (!isSprint)
            {
                bool hasMovement = direction.magnitude >= 0.2f;

                if (hasMovement)
                {
                    if (!isMoving)
                    {
                        isMoving = true;

                        if (IsCrounch)
                        {
                            OnStartCrouching?.Invoke();
                        }
                        else
                        {
                            OnStartWalking?.Invoke();
                        }
                    }
                }
                else
                {
                    if (isMoving)
                    {
                        isMoving = false;
                        OnStopMoving?.Invoke();
                    }
                }
            }

            //if (!isSprint)
            //{
            //    if (direction.magnitude >= 0.2f)
            //    {
            //        if (IsCrounch)
            //        {
            //            isSprint = false;
            //            isCrounch = true;
            //            isMoving = true;

            //            OnStartCrouching?.Invoke();
            //        }
            //        else
            //        {
            //            isSprint = false;
            //            isCrounch = false;
            //            isMoving = true;

            //            OnStartWalking?.Invoke();
            //        }
            //    }
            //    else
            //    {
            //        isMoving = false;
            //        OnStopMoving?.Invoke();
            //    }
            //}

            if (isSprint)
            {
                if (clearInput.y > 0)
                    clearInput.y += 1;
                if (clearInput.y < 0)
                    clearInput.y -= 1;

                if (ChangeStaminaAction == null || !ChangeStaminaAction.Invoke(TypeOfActivity.Running))
                {
                    CanceledSprint();
                }
                if (direction.magnitude < 0.1f)
                {
                    CanceledSprint();
                }
            }

            Vector3 finalMove = direction * speedCurrent;

            if (externalVelocity.magnitude > 0.01f)
            {
                finalMove += externalVelocity;

                externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, knockbackDamping * Time.deltaTime);
            }

            controller.Move(finalMove * Time.deltaTime);

            animator.SetCrouch(IsCrounch);

            float targetForward = clearInput.y;

            animatorForward = Mathf.Lerp(
                animatorForward,
                targetForward,
                Time.deltaTime * sprintSpeed
            );

            Vector2 animVector = new Vector2(
                clearInput.x,
                animatorForward
            );

            animator.SetSpeed(animVector);
        }

        public void Crounch() 
        {
            if (!IsCrounch)
            {
                if (switchSpeedCoroutine != null)
                {
                    StopCoroutine(switchSpeedCoroutine);
                    switchSpeedCoroutine = null;
                }

                controller.height = heightOnCrounch;

                controller.center = new Vector3(
                    baseCenter.x,
                    heightOnCrounch / 2f,
                    baseCenter.z
                );

                cameraCharacter.localPosition = new Vector3(
                    cameraCharacter.localPosition.x,
                    heightOnCrounch,
                    cameraCharacter.localPosition.z
                );
                CanceledSprint();
                StartCoroutine(SwitchStateSpeed(crounchSpeed));
            }
            else
            {
                if (switchSpeedCoroutine != null)
                {
                    StopCoroutine(switchSpeedCoroutine);
                    switchSpeedCoroutine = null;
                }

                controller.height = baseHeight;
                controller.center = baseCenter;
                cameraCharacter.localPosition = baseCameraPosition;

                StartCoroutine(SwitchStateSpeed(walkSpeed));
            }

            isCrounch = !IsCrounch;
        }

        public void Sprint()
        {
            if (direction.magnitude < 0.1f || IsCrounch)
            {
                return;
            }

            if (ChangeStaminaAction == null)
                return;

            isSprint = true;

            if (switchSpeedCoroutine != null)
                StopCoroutine(switchSpeedCoroutine);

            switchSpeedCoroutine = StartCoroutine(SwitchStateSpeed(sprintSpeed));

            OnStartSprinting?.Invoke();
        }

        public void CanceledSprint()
        {
            if (!isSprint)
                return;

            isSprint = false;

            if (switchSpeedCoroutine != null)
            {
                StopCoroutine(switchSpeedCoroutine);
                switchSpeedCoroutine = null;
            }

            float targetSpeed = IsCrounch ? crounchSpeed : walkSpeed;
            switchSpeedCoroutine = StartCoroutine(SwitchStateSpeed(targetSpeed));

            OnStopSprinting?.Invoke();
        }

        private IEnumerator SwitchStateSpeed(float newValue)
        {
            float startSpeed = speedCurrent;
            float elapsedTime = 0f;

            while (elapsedTime < durationTransitionSpeed)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / durationTransitionSpeed;

                speedCurrent = Mathf.Lerp(startSpeed, newValue, t);

                yield return null;
            }
            speedCurrent = newValue;
            switchSpeedCoroutine = null;
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }
    }
}
