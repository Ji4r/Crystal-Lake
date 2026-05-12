using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MyProj
{
    class UiStaminaAnimation : IDisposable
    {
        private float duration;
        private bool isEnableStamina;
        Sequence sequence;

        public UiStaminaAnimation(float duration)
        {
            this.duration = duration;
        }

        public async UniTask HideStaminaBar(Image staminaImage)
        {
            sequence = DOTween.Sequence();
            sequence.SetLink(staminaImage.gameObject);
            sequence.Join(staminaImage.DOFade(0, duration));

            await sequence.AsyncWaitForCompletion();
            isEnableStamina = false;
        }

        public void ShowStaminaBar(Image staminaImage)
        {
            if (isEnableStamina)
                return;

            isEnableStamina = !isEnableStamina;

            if (sequence != null)
            {
                sequence.Kill();
            }

            staminaImage.color = new Color(staminaImage.color.r, staminaImage.color.g,
                staminaImage.color.b, 1);
        }

        public void Dispose()
        {
            if (sequence != null)
            {
                sequence.Kill();
            }
        }
    }
}
