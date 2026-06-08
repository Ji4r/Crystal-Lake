using TriInspector;
using UnityEngine;

namespace MyProj
{
    [System.Serializable]
    public class CardAnimation
    {
        [Tooltip("Card parameters")]
        public string NameTriggeer;
        public Sprite SpriteIcon;
    }

    public class CircularAnimationMenu : MonoBehaviour, IPartPlayer
    {
        [Title("Settings for circular menu")]
        [SerializeField] private Transform parentCircleCard;
        [SerializeField] private CardAnimation[] cardAnimations;
        [SerializeField] private UiCardTabAnimation prefabCard;
        [SerializeField] private float radius = 360f;

        [Title("Reference for script")]
        [SerializeField] private AllPartPlayer allPartPlayer;

        private CharacterUiView characterUiView;
        private CharacterAnimator animator;

        private void Awake()
        {
            int count = cardAnimations.Length;

            for (int i = 0; i < count; i++)
            {
                CardAnimation animation = cardAnimations[i];

                UiCardTabAnimation card =
                    Instantiate(prefabCard, parentCircleCard);

                float angle = i * Mathf.PI * 2f / count;

                Vector2 position = new Vector2(
                    Mathf.Cos(angle),
                    Mathf.Sin(angle)
                ) * radius;

                card.GetComponent<RectTransform>().anchoredPosition = position;

                card.SetSprite(animation.SpriteIcon);

                card.AddActionOnButton(BtnAnims, animation.NameTriggeer);
            }
        }

        private void Start() 
        {
            characterUiView = allPartPlayer.Get<CharacterUiView>();
            animator = allPartPlayer.Get<CharacterAnimator>();
        }

        public void OpenTabCircle()
        {
            characterUiView.ShowTabCircle();
        }

        public void CloseTabCircle()
        {
            characterUiView.HideTabCircle();
        }

        private void BtnAnims(string str)
        {
            CloseTabCircle();
            animator.SetTabAnims(str);
        }
    }
}
