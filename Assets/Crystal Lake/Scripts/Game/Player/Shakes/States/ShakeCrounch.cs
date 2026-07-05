using UnityEngine;

namespace MyProj
{
    public class ShakeCrounch : ShakeState
    {
        public ShakeCrounch(Camera cam, ScriptableShakeCamera preset, float startLocalY) : base(cam, preset, startLocalY)
        {
        }
    }
}
