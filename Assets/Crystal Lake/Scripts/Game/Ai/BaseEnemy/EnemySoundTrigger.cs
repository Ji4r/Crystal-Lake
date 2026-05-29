using Mirror;
using System;
using UnityEngine;

namespace MyProj
{
    public class EnemySoundTrigger : NetworkBehaviour
    {
        public event Action<Vector3> OnPlayerDetected;
        public event Action OnPlayerLeft;

        public void EnterOnTrigger(Vector3 position)
        {
            if (isServer)
            {
                OnPlayerDetected?.Invoke(position);
            }
        }

        public void ExitOnTrigger()
        {
            if (isServer)
            {
                OnPlayerLeft?.Invoke();
            }
        }
    }
}
