using Cysharp.Threading.Tasks;
using System;

namespace MyProj
{
    public abstract class Effect : IDisposable
    {
        public float DurationEffect;

        public virtual async UniTask EnableEffect() { }
        public virtual async UniTask DissableEffect() { }
        public virtual async UniTask AddDurationTimeEffect(float duration) { }
        public abstract void Dispose();
    }
}
