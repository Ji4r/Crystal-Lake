using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

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

        public void AddEffects<T>(Func<T> factory) where T : Effect
        {
            var effect = factory();

            var type = effect.GetType();

            if (listEffects.TryGetValue(type, out var foundEffect))
            {
                foundEffect.AddDurationTimeEffect(effect.DurationEffect).Forget();
                return;
            }

            listEffects.Add(type, effect);
            effect.EnableEffect().Forget();
        }

        public void DeleteEffect<T>() where T : Effect
        {
            DeleteEffect(typeof(T));
        }

        public void DeleteEffect(Effect effect)
        {
            DeleteEffect(effect.GetType());
        }

        private void DeleteEffect(Type type)
        {
            if (listEffects.TryGetValue(type, out var effect))
            {
                effect.Dispose();
                listEffects.Remove(type);
                return;
            }

            throw new Exception($"Эффект {type} не найден");
        }
    }
}
