using DG.Tweening;
using UnityEngine;

namespace MyProj
{
    public enum DirectionMoveText
    {
        Left,
        Right
    }

    [RequireComponent(typeof(HandlerButton))]
    public class AnimsButtonMoverText : MonoBehaviour, IAnimsButton
    {
        [SerializeField] private ScriptableButton presetAnims;

        [SerializeField] private RectTransform btnTextTransform;
        [SerializeField] private DirectionMoveText directionMoveText;

        private Vector3 basePosition;
        private Vector3 offsetPsition;
        private Tween animsMove;


        private void Awake()
        {
            basePosition = btnTextTransform.localPosition;

            offsetPsition = basePosition;
            offsetPsition.x += directionMoveText == DirectionMoveText.Left ? -presetAnims.AddPositionByX : presetAnims.AddPositionByX;
        }

        public void OnEnter()
        {
            KillAnims();

            animsMove = btnTextTransform.DOLocalMove(offsetPsition, presetAnims.DurationOffsetsPositionByX).SetEase(presetAnims.EaseMoveHover)
                .SetLink(btnTextTransform.gameObject);
        }

        public void OnExit()
        {
            KillAnims();

            animsMove = btnTextTransform.DOLocalMove(basePosition, presetAnims.DurationOffsetsPositionByX).SetEase(presetAnims.EaseMoveHover)
                .SetLink(btnTextTransform.gameObject);
        }

        public void OnDown()
        {
            //KillAnims();

        }

        public void OnUp()
        {
            //KillAnims();

        }


        private void KillAnims()
        {
            if (animsMove != null)
                animsMove.Kill();
        }

        private void OnDisable()
        {
            KillAnims();
        }
    }
}
