using UnityEngine;
using Zenject;

namespace MyProj
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioLowPassFilter))]
    public class VoiceOcclusion : MonoBehaviour, ILocalOnly
    {
        private VoiceSettings settings;

        private AudioSource source;
        private AudioLowPassFilter lowPass;

        private Transform listener;

        [Inject]
        private void Construct(VoiceSettings settings)
        {
            this.settings = settings;
        }

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            lowPass = GetComponent<AudioLowPassFilter>();
        }

        private void Start()
        {
            var audioListener = FindFirstObjectByType<AudioListener>();

            if (audioListener != null)
            {
                listener = audioListener.transform;
            }
        }

        private void Update()
        {
            if (listener == null)
                return;

            bool blocked = Physics.Linecast(
                transform.position,
                listener.position,
                settings.WallLayer);

            lowPass.cutoffFrequency =
                blocked
                ? settings.WallCutoffFrequency
                : settings.NormalCutoffFrequency;
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }
    }
}
