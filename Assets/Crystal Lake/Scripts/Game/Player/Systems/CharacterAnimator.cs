using UnityEngine;

namespace MyProj
{
    public class CharacterAnimator : MonoBehaviour, ILocalOnly
    {
        [SerializeField] private Animator animator;

        public void SetSpeed(Vector2 moveSpeed)
        {
            animator.SetFloat("_x", moveSpeed.x);
            animator.SetFloat("_z", moveSpeed.y);
        }

        public void SetSpeedByY(float moveSpeed)
        {
            animator.SetFloat("_y", moveSpeed);
        }

        public void SetJump()
        {
            animator.SetTrigger("_jump");
        }

        public void SetCrouch(bool isCrouching)
        {
            animator.SetBool("_isCrouch", isCrouching);
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }
    }
}
