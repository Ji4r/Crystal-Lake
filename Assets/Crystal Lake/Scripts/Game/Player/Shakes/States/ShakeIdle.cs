using UnityEngine;

namespace MyProj
{
    public class ShakeIdle : ShakeState
    {
        public ShakeIdle(Camera cam, ScriptableShakeCamera preset, float startLocalY) : base(cam, preset, startLocalY)
        {
        }
    }
}
