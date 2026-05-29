using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

namespace MyProj
{
    public class Button3DAnimator : IDisposable
    {
        private readonly ScriptableButton3D preset;
        private Tween currentTween;

        public Button3DAnimator(ScriptableButton3D preset)
        {
            this.preset = preset;
        }

        public void Dispose()
        {
            if (currentTween != null)
            {
                currentTween.Kill();
                currentTween = null;
            }
        }

        public async UniTask AnimatePress(Transform buttonTransform, Direction dir)
        {
            if (buttonTransform == null)
                throw new ArgumentNullException(nameof(buttonTransform));

            Dispose();

            Vector3 endPosition = buttonTransform.localPosition + GetDirection(dir);

            currentTween = buttonTransform.DOLocalMove(endPosition, preset.DepthDuration)
                .SetEase(preset.Ease).SetLink(buttonTransform.gameObject);

            await currentTween.AsyncWaitForCompletion();
        }

        public async UniTask AnimateRelease(Transform buttonTransform, Vector3 basePosition)
        {
            if (buttonTransform == null)
                throw new ArgumentNullException(nameof(buttonTransform));

            Dispose();

            currentTween = buttonTransform.DOLocalMove(basePosition, preset.ReleasedDuration)
                .SetEase(preset.Ease).SetLink(buttonTransform.gameObject);

            await currentTween.AsyncWaitForCompletion();
        }


        private Vector3 GetDirection(Direction dir)
        {
            return dir switch
            {
                Direction.Forward => Vector3.forward * preset.PressDepth,
                Direction.Backward => Vector3.back * preset.PressDepth,
                Direction.Up => Vector3.up * preset.PressDepth,
                Direction.Down => Vector3.down * preset.PressDepth,
                Direction.Left => Vector3.left * preset.PressDepth,
                Direction.Right => Vector3.right * preset.PressDepth,
                _ => Vector3.zero
            };
        }
    }
}
