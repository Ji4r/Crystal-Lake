using DG.Tweening;
using System;
using UnityEngine;

namespace MyProj
{
    public abstract class ShakeState : IDisposable
    {
        protected Transform playerCamera;
        protected ScriptableShakeCamera preset;
        protected Tween currentShake;

        public bool isPlay;
        private float startLocalY; 

        public ShakeState(Camera cam, ScriptableShakeCamera preset, float startLocalY)
        {
            playerCamera = cam.transform;
            this.preset = preset;
            this.startLocalY = startLocalY;
        }

        public virtual void Dispose()
        {
           if (currentShake != null)
                currentShake.Kill();
           isPlay = false;
        }

        public virtual void EnabledShake()
        {
            isPlay = true;
            currentShake = playerCamera.DOLocalMoveY(startLocalY + preset.Power, preset.Duration)
                .SetEase(Ease.InOutQuad)
                .SetLoops(-1, LoopType.Yoyo);

            //playerCamera.transform.DOLocalMoveX(preset.Power * 0.5f, preset.Duration * 2f)
            //    .From(-preset.Power * 0.5f)
            //    .SetEase(Ease.InOutQuad)
            //    .SetLoops(-1, LoopType.Yoyo);
        }

        public virtual void DissabledShake()
        {
            Dispose();
        }
    }
}