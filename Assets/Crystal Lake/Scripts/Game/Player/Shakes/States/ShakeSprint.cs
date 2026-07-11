using UnityEngine;

namespace MyProj
{
    public class ShakeSprint : ShakeState
    {
        public ShakeSprint(Camera cam, ScriptableShakeCamera preset, float startLocalY) : base(cam, preset, startLocalY)
        {
        }
    }
}
