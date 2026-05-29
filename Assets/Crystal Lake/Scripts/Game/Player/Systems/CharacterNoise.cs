using Mirror;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

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

        private Transform myTransform;
        private Dictionary<NoiseType, float> noiseRadiusDictionary;

        private float multipleNoise;

        [Inject]
        public void Construct()
        {
            multipleNoise = DifficultyGame.Instance.Current.PlayersNoiseMultiplier;
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