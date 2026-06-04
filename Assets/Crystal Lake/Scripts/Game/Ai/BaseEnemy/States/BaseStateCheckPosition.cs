using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using System.Threading;

namespace MyProj
{
    public class BaseStateCheckPosition : FsmAIEnemyState // проверка места
    {
        private float distancetoPointOnNoise;
        private CancellationTokenSource cancellationTokenSource;
        private int whatTimeIsWatchingSide;

        public BaseStateCheckPosition(BaseEnemy enemy, float distanceToNoise, float whatTimeIsWatchingSide) : base(enemy)
        {
            this.distancetoPointOnNoise = distanceToNoise;
            this.whatTimeIsWatchingSide = (int)whatTimeIsWatchingSide * 1000;
        }

        public override void EnterState()
        {
            Debug.Log("CheckPosition");
            cancellationTokenSource = new CancellationTokenSource();
            enemy.SoundTrigger.OnPlayerDetected += OnPlayerDetectedThroughSound;
            enemy.SetDestination(enemy.CheckPosition);
            CheckPose(cancellationTokenSource.Token).Forget();
        }

        public override void ExitState()
        {
            enemy.SoundTrigger.OnPlayerDetected -= OnPlayerDetectedThroughSound;
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }

        public override void UpdateState()
        {
            var state = enemy.TryFindTargetCached(out var currentTarget);
            if (state == StateVisionEnemy.HadSeen)
            {
                enemy.SetState(EnemyState.Chase);
                return;
            }
            else if (state == StateVisionEnemy.ByHalf && currentTarget != null)
            {
                enemy.CheckPosition = currentTarget.position;
                EnterState();
                return;
            }
            else if (state == StateVisionEnemy.None)
            {
                foreach (var noise in enemy.SoundTrigger.MemoryEnemies)
                {
                    if (noise.ExpireTime > Time.time)
                    {
                        enemy.CheckPosition = noise.NoisePosition;
                        enemy.SetState(EnemyState.Searching);
                        return;
                    }
                }

                enemy.SetState(EnemyState.Patrol);
                return;
            }
        }

        private async UniTask CheckPose(CancellationToken token)
        {
            try
            {
                await UniTask.WaitUntil(() =>
                {
                    Vector3 dir =
                        enemy.MyTransform.position -
                        enemy.NoisePosition;

                    return dir.sqrMagnitude <=
                           distancetoPointOnNoise *
                           distancetoPointOnNoise;

                }, cancellationToken: token);

                await RotateTo(enemy.RadiusRotateHeadRight, token);

                await UniTask.Delay(whatTimeIsWatchingSide, cancellationToken: token);

                await RotateTo(enemy.RadiusRotateHeadLeft - enemy.RadiusRotateHeadLeft, token);

                await UniTask.Delay(whatTimeIsWatchingSide, cancellationToken: token);

                enemy.SetState(EnemyState.Patrol);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async UniTask RotateTo(float angle, CancellationToken token)
        {
            Quaternion startRotation = enemy.MyTransform.rotation;

            Quaternion targetRotation =
                Quaternion.Euler(
                    0,
                    startRotation.eulerAngles.y + angle,
                    0);

            while (Quaternion.Angle(
                       enemy.MyTransform.rotation,
                       targetRotation) > 0.5f)
            {
                enemy.MyTransform.rotation =
                    Quaternion.RotateTowards(
                        enemy.MyTransform.rotation,
                        targetRotation,
                        120f * Time.deltaTime);

                await UniTask.Yield(token);
            }
        }

        private void OnPlayerDetectedThroughSound(Vector3 vector)
        {

            enemy.SetNoisePosition(vector);
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
            enemy.SetDestination(vector);

            cancellationTokenSource =
                new CancellationTokenSource();

            CheckPose(cancellationTokenSource.Token).Forget();
        }
    }
}
