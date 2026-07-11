using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public class BurpEffect : Effect
    {
        private int multiplicationToSeconds; // Домножение до секунд 
        private EffectsOnPlayer effectsOnPlayer;
        private CharacterNoise characterNoise;

        private Queue<float> queueBurp;


        public BurpEffect(EffectsOnPlayer effectsOnPlayer, CharacterNoise characterNoise, float effectDuration)
        {
            queueBurp = new Queue<float>();
            multiplicationToSeconds = 1000;
            this.DurationEffect = effectDuration * multiplicationToSeconds;
            this.effectsOnPlayer = effectsOnPlayer;
            this.characterNoise = characterNoise;
        }

        public override async UniTask EnableEffect()
        {
            await UpdateEffect();
        }

        private async UniTask UpdateEffect() 
        {
            int burpingThrough = Random.Range(0, (int)DurationEffect);
            await UniTask.Delay(burpingThrough);
            characterNoise?.MakeNoise(NoiseType.Burp);
            Debug.LogWarning($"Burp - {burpingThrough} - звук отрыжки нужно добавить");

            if (queueBurp.Count != 0)
            {
                queueBurp.Dequeue();
                await UpdateEffect();
                return;
            }

            await DissableEffect();
        }

        public override async UniTask AddDurationTimeEffect(float duration)
        {
            await base.AddDurationTimeEffect(duration);
            queueBurp.Enqueue(duration);
        }

        public override async UniTask DissableEffect()
        {
            Debug.Log("Dissable BurpEffect");
            effectsOnPlayer?.DeleteEffect(this);
        }

        public override void Dispose()
        {
            
        }
    }
}
