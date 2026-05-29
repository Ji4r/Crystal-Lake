using UnityEngine;

namespace MyProj
{
    public class CharacterHandAnimator : MonoBehaviour, IPartPlayer
    {
        [SerializeField] private Animator animator; 

        public void SetUpHand()
        {
            animator.SetTrigger("_upHand");
        }

        public void SetReturnHand()
        {
            animator.SetTrigger("_downHand");
        }
    }
}
