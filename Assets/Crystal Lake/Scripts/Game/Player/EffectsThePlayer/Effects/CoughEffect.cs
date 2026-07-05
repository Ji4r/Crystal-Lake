using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace MyProj
{
    public class CoughEffect : Effect
    {
        private EffectsOnPlayer effectsOnPlayer;
        private CharacterNoise characterNoise;
        private RangeFloat coughResponseTime;

        private CancellationTokenSource cancellationTokenSource;

        public CoughEffect(EffectsOnPlayer effectsOnPlayer, CharacterNoise characterNoise, float effectDuration, RangeFloat coughResponseTime)
        {
            this.DurationEffect = effectDuration;
            this.effectsOnPlayer = effectsOnPlayer;
            this.characterNoise = characterNoise;
            this.coughResponseTime = coughResponseTime;
        }

        public override async UniTask EnableEffect()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
            }

            cancellationTokenSource = new CancellationTokenSource();

            Updater(cancellationTokenSource.Token).Forget();

            await UniTask.Yield();
        }

        private async UniTask Updater(CancellationToken token)
        {
            float endTime = DurationEffect + Time.time; 
            while (Time.time < endTime)
            {
                try
                {
                    if (endTime < Time.time)
                    {
                        DurationEffect = 0;
                    }
                    else
                    {
                        DurationEffect = endTime - Time.time;
                    }

                    await UniTask.Delay(
                        (int)UnityEngine.Random.Range(
                            coughResponseTime.minValue, 
                            coughResponseTime.maxValue) 
                        * 1000, 
                        cancellationToken: token);

                    Effect();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }

            DurationEffect = 0;
            await UpdateEffect();
        }

        private async UniTask UpdateEffect()
        {
            await DissableEffect();
        }

        public override async UniTask AddDurationTimeEffect(float duration)
        {
            DurationEffect += duration;
            await EnableEffect();
        }

        public override async UniTask DissableEffect()
        {
            effectsOnPlayer?.DeleteEffect(this);
        }

        public override void Dispose()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

        private void Effect()
        {
            characterNoise?.MakeNoise(NoiseType.Cough);
            Debug.LogWarning($"Cough - звук кашля нужно сюдамс");
        }
    }
}