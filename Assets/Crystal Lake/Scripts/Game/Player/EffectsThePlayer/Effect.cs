using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


namespace MyProj
{
    public abstract class Effect : IDisposable
    {
        public float DurationEffect;

        public virtual async UniTask EnableEffect() 
        {
            Debug.Log("EnableEffect");
            await UniTask.CompletedTask;
        }

        public virtual async UniTask DissableEffect()
        {
            Debug.Log("DissableEffect");
            await UniTask.CompletedTask;
        }

        public virtual async UniTask AddDurationTimeEffect(float duration)
        {
            Debug.Log("AddDurationTimeEffect");
            await UniTask.CompletedTask;
        }

        public abstract void Dispose();
    }
}
