using Mirror;
using System;
using UnityEngine;

namespace MyProj
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterGravity : NetworkBehaviour, ILocalOnly
    {
        [Header("Право созидателя")]
        [SerializeField] private bool infinityJumpEditor;
        [SerializeField] private float jumpForceEditor;

        [Header("Обычные настройки")]
        [SerializeField] private AllPartPlayer allPartPlayer;

        [Header("Настройки прыжка")]
        [SerializeField, Tooltip("Гравитация")] 
        private float gravity = -9.81f;
        [SyncVar, SerializeField, Tooltip("Сила прыжка")] 
        private float jumpForce = 4f;
        [SerializeField, Tooltip("При каком растоянии от земли игрок сможет прыгать")] 
        private float distanceGrounded = 0.15f;
        [SerializeField, Tooltip("На каких поверхностях игрок сможет прыгать")] 
        private LayerMask grounded;

        [Header("Урон от падения")]
        [SerializeField, Tooltip("Со скольки метров персонаж начнёт получать урон")]
        private float distanceFolling;
        [SerializeField, Tooltip("Урон при падении")]
        private float demageAtFolling;

        public event Func<TypeOfActivity, bool> ChangeStaminaAction;
        public event Action<byte> ChangeHealthAction;

        public bool IsGrounded => onGround;

        [SyncVar] private bool isInfinityJump;
        private CharacterNoise noise;
        private Vector3 velocity;
        private bool onGround;
        private CharacterController controller;
        private float startFallHeight;
        private bool wasGrounded;
        CharacterAnimator animator;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            isInfinityJump = infinityJumpEditor;
            jumpForce = jumpForceEditor;
        }
#endif

        private void Awake()
        {
            noise = allPartPlayer.Get<CharacterNoise>();
            animator = allPartPlayer.Get<CharacterAnimator>();
        }

        private void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            bool prevGrounded = onGround;
            onGround = CheckIsGrounded();

            if (!onGround && prevGrounded)
            {
                startFallHeight = transform.position.y;
            }

            if (onGround && !prevGrounded)
            {
                float fallDistance = startFallHeight - transform.position.y;

                if (fallDistance > distanceFolling)
                {
                    float damage = (fallDistance - distanceFolling) * demageAtFolling;
                    //Debug.Log($"Урон от падения: {damage}");
                    ChangeHealthAction?.Invoke((byte)Mathf.Round(damage));
                }
            }
        }

        private void LateUpdate()
        {
             UseGravity();
        }

        public void Jump()
        {
            if (!isInfinityJump && !onGround)
                return;

            if (!isInfinityJump)
                if (ChangeStaminaAction == null || !ChangeStaminaAction.Invoke(TypeOfActivity.Jumping))
                    return;

            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            controller.Move(velocity * Time.deltaTime);
            noise.MakeNoise(NoiseType.Jump);
            animator.SetJump();
        }

        private bool CheckIsGrounded()
        {
            Vector3 spherePosition = new Vector3(
                controller.bounds.center.x,
                controller.bounds.min.y + 0.05f,
                controller.bounds.center.z
            );

            return Physics.CheckSphere(
                spherePosition,
                distanceGrounded,
                grounded,
                QueryTriggerInteraction.Ignore);

            //Vector3 spherePosition = transform.position + Vector3.down * (controller.height / 2);
            //return Physics.CheckSphere(spherePosition, distanceGrounded, grounded);
        }

        private void UseGravity()
        {
            if (onGround && velocity.y < 0)
            {
                velocity.y = -1;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }

            animator.SetSpeedByY(velocity.y);
            controller.Move(velocity * Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            if (controller == null)
                controller = GetComponent<CharacterController>();

            Vector3 spherePosition = new Vector3(
                controller.bounds.center.x,
                controller.bounds.min.y + 0.05f,
                controller.bounds.center.z
            );

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spherePosition, distanceGrounded);
            //if (controller == null) return;

            //Vector3 spherePosition = transform.position + Vector3.down * (controller.height / 2);

            //Gizmos.color = Color.green;
            //Gizmos.DrawWireSphere(spherePosition, distanceGrounded);
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }
    }
}