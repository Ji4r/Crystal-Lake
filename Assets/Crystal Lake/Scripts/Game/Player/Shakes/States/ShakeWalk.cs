using UnityEngine;

namespace MyProj
{
    public class ShakeWalk : ShakeState
    {
        public ShakeWalk(Camera cam, ScriptableShakeCamera preset, float startLocalY) : base(cam, preset, startLocalY)
        {
        }
    }
}
