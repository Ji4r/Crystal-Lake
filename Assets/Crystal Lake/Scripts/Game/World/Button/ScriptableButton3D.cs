using DG.Tweening;
using UnityEngine;

namespace MyProj
{
    [CreateAssetMenu(fileName = "3DButtonPreset", menuName = "SO/World/3DButton")]
    public class ScriptableButton3D : ScriptableObject
    {
        [SerializeField] private float pressDepth = 0.1f;
        [SerializeField] private float depthDuration = 0.25f;
        [SerializeField] private float releasedDuration = 0.25f;
        [SerializeField] private Ease ease = Ease.Linear;

        public float PressDepth { get => pressDepth; }
        public float DepthDuration { get => depthDuration; }
        public float ReleasedDuration { get => releasedDuration; }
        public Ease Ease { get => ease; }
    }
}
