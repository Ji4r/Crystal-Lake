using Cysharp.Threading.Tasks;
using Mirror;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MyProj
{
    public class EnemyNoiseMemory
    {
        public Vector3 NoisePosition;
        public float ExpireTime;
    }

    public class EnemySoundTrigger : NetworkBehaviour
    {
        [SerializeField, Tooltip("Время, на которое запоминается позиция шума")]
        private float timeMemoryNoise = 20f;

        public event Action<Vector3> OnPlayerDetected;
        public event Action OnPlayerLeft;

        public float TimeMemoryNoise => timeMemoryNoise;
        public List<EnemyNoiseMemory> MemoryEnemies { get; private set; }

        private CancellationTokenSource cancellationTokenSource;

        public override void OnStartServer()
        {
            MemoryEnemies = new List<EnemyNoiseMemory>(10);
            cancellationTokenSource = new CancellationTokenSource();
            GarbedgeCollectorMemory(cancellationTokenSource.Token).Forget();
        }

        private void OnDisable()
        {
            if (isServer)
            {
                MemoryEnemies.Clear();
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
            }
        }

        public void EnterOnTrigger(Vector3 position)
        {
            if (isServer)
            {
                MemoryEnemies.Add(new EnemyNoiseMemory 
                {   
                    NoisePosition = position, 
                    ExpireTime = Time.time + timeMemoryNoise
                });

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

        private async UniTask GarbedgeCollectorMemory(CancellationToken cancellationToken)
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);

                if (MemoryEnemies.Count <= 0)
                {
                    continue;
                }

                for (int i = MemoryEnemies.Count - 1; i >= 0; i--)
                {
                    if (MemoryEnemies[i].ExpireTime < Time.time)
                    {
                        MemoryEnemies.RemoveAt(i);
                    }
                }
            }
        }
    }
}
