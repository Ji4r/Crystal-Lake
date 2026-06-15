using UnityEngine;
using TriInspector;

namespace MyProj
{
    [CreateAssetMenu(fileName = "VoiceSettings", menuName = "SO/VoiceSettings")]
    public class VoiceSettings : ScriptableObject
    {
        [Title("Voice Chat")]
        [SerializeField] private float minDistance = 3f;
        [SerializeField] private float maxDistance = 20f;

        [Title("Voice Noise")]
        [SerializeField] private float voiceThreshold = 0.02f;
        [SerializeField] private float noiseMultiplier = 300f;
        [SerializeField] private float minNoiseRadius = 2f;
        [SerializeField] private float maxNoiseRadius = 40f;

        [Title("Voice Update")]
        [SerializeField] private float noiseInterval = 0.25f;

        [Title("Menu")]
        [SerializeField, Range(0f, 1f)] private float spatialBlendInMenu = 0f;

        [Title("Game")]
        [SerializeField, Range(0f, 1f)] private float spatialBlendInGame = 1f;
        [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;
        [SerializeField] private float dopplerLevel = 0f;

        [Title("Occlusion in Game")]
        [SerializeField] private float wallVolumeMultiplier = 0.3f;
        [SerializeField] private float wallLowPassFrequency = 1000f;

        [Title("Occlusion")]
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private float wallCutoffFrequency = 800f;
        [SerializeField] private float normalCutoffFrequency = 22000f;


        public float MinDistance => minDistance;
        public float MaxDistance => maxDistance;
        public float VoiceThreshold => voiceThreshold;
        public float NoiseMultiplier => noiseMultiplier;
        public float MinNoiseRadius => minNoiseRadius;
        public float MaxNoiseRadius => maxNoiseRadius;
        public float NoiseInterval => noiseInterval;

        public float SpatialBlendInMenu => spatialBlendInMenu;
        public float SpatialBlendInGame => spatialBlendInGame;
        public AudioRolloffMode RolloffMode => rolloffMode;
        public float DopplerLevel=> dopplerLevel;
        public float WallVolumeMultiplier => wallVolumeMultiplier; 
        public float WallLowPassFrequency => wallLowPassFrequency;
        public LayerMask WallLayer => wallLayer;
        public float WallCutoffFrequency => wallCutoffFrequency;
        public float NormalCutoffFrequency => normalCutoffFrequency;
    }
}
