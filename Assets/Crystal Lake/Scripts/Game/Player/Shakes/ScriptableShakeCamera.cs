using UnityEngine;

namespace MyProj
{
    [CreateAssetMenu(fileName = "ScriptableShakeCamera", menuName = "SO/Anims/ShakeCamera")]
    public class ScriptableShakeCamera : ScriptableObject
    {
        [SerializeField] private float duration;
        [SerializeField] private float power;

        public float Duration { get => duration; }
        public float Power { get => power; }
    }
}
