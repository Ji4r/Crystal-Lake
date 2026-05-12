using System;
using UnityEngine;
using System.Collections.Generic;

namespace MyProj
{
    public class EffectsOnPlayer : MonoBehaviour, IPartPlayer
    {
        [SerializeField] private AllPartPlayer allPartPlayer;

        public Dictionary<Type, Effect> listEffects;

        private void Start()
        {
            listEffects = new();
        }


        public void StartEffects<T>(Func<T> factory) where T : Effect
        {
            var effect = factory();

            var type = effect.GetType();

            if (listEffects.TryGetValue(type, out var foundEffect))
            {
                foundEffect.AddEffectTime();
                return;
            }

            listEffects.Add(type, effect);
            effect.EnableEffect();
        }

        public void DisableEffect<T>() where T : Effect
        {
            var type = typeof(T);

            if (listEffects.TryGetValue(type, out var effect))
            {
                effect.DissableEffect();
                listEffects.Remove(type);
                return;
            }

            throw new Exception($"Эффект - {type} не найден");
        }
    }
}
