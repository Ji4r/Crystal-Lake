using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MyProj
{
    public class MoverUIFriendList : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform panelFriendList;
        [SerializeField] private Button btnMove;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Transform transformChildrenBtnFriendList;

        [Header("Animation")]
        [SerializeField] private float durationAnimChildrenBtnFriendList = 0.4f;
        [SerializeField] private float duration = 0.35f;

        [SerializeField] private float hiddenX = 420f;
        [SerializeField] private float visibleX = 0f;

        private bool isOpened;
        private Tween currentTween;

        private void Start()
        {
            panelFriendList.anchoredPosition =
                new Vector2(hiddenX, panelFriendList.anchoredPosition.y);

            if (canvasGroup != null)
                canvasGroup.alpha = 0f;

            isOpened = false;
        }

        private void OnEnable()
        {
            btnMove.onClick.AddListener(OnMoveButtonClicked);
        }

        private void OnDisable()
        {
            btnMove.onClick.RemoveListener(OnMoveButtonClicked);
        }

        private void OnMoveButtonClicked()
        {
            currentTween?.Kill();

            float targetX = isOpened ? hiddenX : visibleX;
            float targetAlpha = isOpened ? 0f : 1f;

            Sequence sequence = DOTween.Sequence();

            // Slide animation
            sequence.Join(
                panelFriendList.DOAnchorPosX(targetX, duration)
                    .SetEase(Ease.OutCubic)
            ).Join(transformChildrenBtnFriendList.DOLocalRotate(isOpened == true ? new Vector3(0, 0, 0) : new Vector3(0, 0, 180),
            durationAnimChildrenBtnFriendList));

            // Fade animation
            if (canvasGroup != null)
            {
                sequence.Join(
                    canvasGroup.DOFade(targetAlpha, duration * 0.9f)
                );
            }

            // Лёгкий overshoot как в игровых UI
            sequence.Join(
                panelFriendList.DOScale(
                    isOpened ? 0.98f : 1f,
                    duration
                ).SetEase(Ease.OutQuad)
            );

            currentTween = sequence;

            isOpened = !isOpened;
        }
    }
}