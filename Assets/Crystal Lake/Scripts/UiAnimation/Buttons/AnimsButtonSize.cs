using DG.Tweening;
using UnityEngine;

namespace MyProj
{
    [RequireComponent(typeof(HandlerButton))]
    public class AnimsButtonSize : MonoBehaviour, IAnimsButton
    {
        [SerializeField] private ScriptableButton presetAnims;

        private Vector3 baseSize;
        private Transform btnTransform;
        private Tween anims;
        private Vector3 sizeOnHover;
        private Vector3 sizeOnClick;

        private void Awake()
        {
            btnTransform = transform;

            baseSize = btnTransform.localScale;

            sizeOnHover = new Vector3(baseSize.x + presetAnims.AddSizeOnHover, baseSize.y + presetAnims.AddSizeOnHover, baseSize.z);
            sizeOnClick = new Vector3(baseSize.x + presetAnims.AddSizeOnClick, baseSize.y + presetAnims.AddSizeOnClick, baseSize.z);
        }

        public void OnEnter()
        {
            KillAnims();
            
            btnTransform.DOScale(sizeOnHover, presetAnims.DurationAnimsOnHover).SetEase(presetAnims.EaseScheduleHover)
                .SetLink(btnTransform.gameObject);
        }

        public void OnExit()
        {
            KillAnims();

            btnTransform.DOScale(baseSize, presetAnims.DurationAnimsOnHover).SetEase(presetAnims.EaseScheduleHover)
                .SetLink(btnTransform.gameObject); ;
        }

        public void OnDown()
        {
            KillAnims();

            btnTransform.DOScale(sizeOnClick, presetAnims.DurationAnimsOnHover).SetEase(presetAnims.EaseScheduleHover)
                .SetLink(btnTransform.gameObject);
        }

        public void OnUp()
        {
            KillAnims();

            btnTransform.DOScale(sizeOnHover, presetAnims.DurationAnimsOnHover).SetEase(presetAnims.EaseScheduleHover)
                .SetLink(btnTransform.gameObject);
        }


        private void KillAnims()
        {
            if (anims != null)
                anims.Kill();
        }

        private void OnDisable()
        {
            KillAnims();
        }
    }
}
