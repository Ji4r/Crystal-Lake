using Mirror;
using UnityEngine;

namespace MyProj
{
    public class CharacterAnimator : MonoBehaviour, ILocalOnly
    {
        [SerializeField] private Animator animator;
        private bool lastCrouchState;

        [SerializeField] private NetworkAnimator networkAnimator;

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
            CancelEmotion();
            animator.SetTrigger("_jump");
        }

        public void SetCrouch(bool isCrouching)
        {
            if (lastCrouchState != isCrouching)
            {
                if (isCrouching)
                    CancelEmotion();

                lastCrouchState = isCrouching;
            }

            animator.SetBool("_isCrouch", isCrouching);
        }


        public void SetTabAnims(string name)
        {

            animator.ResetTrigger("_cancelEmotion");


            networkAnimator.SetTrigger(name);
            //animator.SetTrigger(name);
        }

        public void CancelEmotion()
        {
            animator.SetTrigger("_cancelEmotion");
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }
    }
}
