using Mirror;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Adrenak.UniVoice.Inputs;

namespace MyProj
{
    public enum NoiseType
    {
        Crouch,
        Walk,
        Run,
        Jump,
    }

    public class CharacterNoise : NetworkBehaviour, IPartPlayer
    {
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float currentDebugRadius;

        [Header("Noise Settings")]
        [SerializeField] private float noiseWalk = 25;
        [SerializeField] private float noiseRun = 10;
        [SerializeField] private float noiseJump = 15;
        [SerializeField] private float noiseCrouch = 5;

        private float lastVoiceNoiseTime;
        private float voiceNoiseInterval;

        private Transform myTransform;
        private Dictionary<NoiseType, float> noiseRadiusDictionary;

        private float multipleNoise;
        private VoiceSettings voiceSettings;

        [Inject]
        private void Construct(VoiceSettings settings)
        {
            voiceSettings = settings;
            voiceNoiseInterval = settings.NoiseInterval;
            multipleNoise = DifficultyGame.Instance.Current.PlayersNoiseMultiplier;
        }

        private void OnEnable()
        {
            UniMicInput.OnVoiceVolume += HandleVoiceVolume;
        }

        private void OnDisable()
        {
            UniMicInput.OnVoiceVolume -= HandleVoiceVolume;
        }

        private void Start()
        {
            myTransform = transform;
            InitializeNoiseDictionary();
        }

        private void InitializeNoiseDictionary()
        {
            noiseRadiusDictionary = new Dictionary<NoiseType, float>()
            {
                { NoiseType.Walk, noiseWalk * multipleNoise },
                { NoiseType.Run, noiseRun * multipleNoise },
                { NoiseType.Jump, noiseJump * multipleNoise },
                { NoiseType.Crouch, noiseCrouch * multipleNoise },
            };
        }

        private void HandleVoiceVolume(float volume)
        {
            if (!isLocalPlayer)
                return;

            if (Time.time - lastVoiceNoiseTime < voiceNoiseInterval)
                return;

            lastVoiceNoiseTime = Time.time;

            float radius = Mathf.Clamp(volume * voiceSettings.NoiseMultiplier, voiceSettings.MinNoiseRadius, voiceSettings.MaxNoiseRadius);

            CmdMakeVoiceNoise(radius);
        }

        public void MakeNoise(NoiseType noiseType)
        {
            if (!isLocalPlayer)
                return;

            CmdMakeNoise(noiseType);
        }

        [Command]
        private void CmdMakeNoise(NoiseType noiseType)
        {
            float radius = GetNoiseRadius(noiseType);
            currentDebugRadius = radius;

            Collider[] hits = Physics.OverlapSphere(
                transform.position,
                radius,
                enemyLayer);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<EnemySoundTrigger>(out var enemy))
                {
                    enemy.EnterOnTrigger(myTransform.position);
                }
            }
        }

        [Command]
        private void CmdMakeVoiceNoise(float radius)
        {
            currentDebugRadius = radius;

            Collider[] hits = Physics.OverlapSphere(
                transform.position,
                radius,
                enemyLayer);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<EnemySoundTrigger>(out var enemy))
                {
                    enemy.EnterOnTrigger(transform.position);
                }
            }
        }

        public float GetNoiseRadius(NoiseType noiseType)
        {
            if (noiseRadiusDictionary.TryGetValue(
                    noiseType,
                    out float radius))
            {
                return radius;
            }

            Debug.LogWarning(
                $"NoiseType {noiseType} not found");

            return 0f;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(
                transform.position,
                currentDebugRadius);
        }
#endif
    }
}