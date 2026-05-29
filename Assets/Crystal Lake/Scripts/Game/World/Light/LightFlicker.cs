using System.Collections;
using UnityEngine;

namespace MyProj
{
    public class LightFlicker : MonoBehaviour
    {
        [Header("Параметры света")]
        [SerializeField, Tooltip("Колличество мерцаний за раз")]
        RangeByte rangeNumberOfFlicks;
        [SerializeField, Tooltip("Частота фликов в секундах")]
        RangeInt rangeFrequencyOfFlicksInSeconds;
        [SerializeField] private RangeFloat delayBetweenFlicks;
        [SerializeField, Min(1f), Tooltip("Множитель затемнения")]
        private float dimmingMultiplier = 3;

        [Header("Компоненты")]
        [SerializeField] private Light lightForFlick;

        private Coroutine coroutineLightFlicker;
        private Coroutine currentFlicks;
        private float baseIntensity;

        private void OnEnable()
        {
            baseIntensity = lightForFlick.intensity;
            coroutineLightFlicker = StartCoroutine(StartLightFlicker());
        }

        private void OnDisable()
        {
            if (coroutineLightFlicker != null)
            {
                StopCoroutine(coroutineLightFlicker);
                coroutineLightFlicker = null;
            }

            if (currentFlicks != null)
            {
                StopCoroutine(currentFlicks);
                currentFlicks = null;
            }
        }

        private IEnumerator StartLightFlicker()
        {
            int betweenFlicks; 
            while (true) 
            {
                betweenFlicks = Random.Range(
                    rangeFrequencyOfFlicksInSeconds.minValue,
                    rangeFrequencyOfFlicksInSeconds.maxValue);

                yield return new WaitForSeconds(betweenFlicks);

                if (currentFlicks == null)
                    currentFlicks = StartCoroutine(StartFlicks());
            }
        }

        private IEnumerator StartFlicks()
        {
            int countFlicks = Random.Range(
                    rangeNumberOfFlicks.minValue,
                    rangeNumberOfFlicks.maxValue);
            float delayFlicks = Random.Range(
                    delayBetweenFlicks.minValue,
                    delayBetweenFlicks.maxValue);

            WaitForSeconds delay = new WaitForSeconds(delayFlicks);

            for (int i = 0; i < countFlicks; i++)
            {
                lightForFlick.intensity = baseIntensity / dimmingMultiplier;
                SoundManager.PlaySound(Sound.FlickerOfLight);
                yield return delay;
                lightForFlick.intensity = baseIntensity;
                yield return delay;
            }

            currentFlicks = null;
        }
    }
}
