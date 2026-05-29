using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace MyProj
{
    public class BaseStateTrafficOnNoise : FsmAIEnemyState // проверка места где был шум
    {
        private EnemySoundTrigger soundTrigger;
        private float distancetoPointOnNoise;
        private CancellationTokenSource cancellationTokenSource;
        private int whatTimeIsWatchingSide;

        public BaseStateTrafficOnNoise(BaseEnemy enemy, EnemySoundTrigger soundTrigger, float distanceToNoise, float whatTimeIsWatchingSide) : base(enemy)
        {
            this.soundTrigger = soundTrigger;
            this.distancetoPointOnNoise = distanceToNoise;
            this.whatTimeIsWatchingSide = (int)whatTimeIsWatchingSide * 1000;
        }

        public override void EnterState()
        {
            Debug.Log("Я что то услышал");
            cancellationTokenSource = new CancellationTokenSource();
            soundTrigger.OnPlayerDetected += OnPlayerDetectedThroughSound;
            enemy.SetDestination(enemy.NoisePosition);
            CheckPose(cancellationTokenSource.Token).Forget();
        }

        public override void ExitState()
        {
            soundTrigger.OnPlayerDetected -= OnPlayerDetectedThroughSound;
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }

        public override void UpdateState()
        {
            if (enemy.TryFindTargetCached())
            {
                enemy.SetState(EnemyState.Chase);
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
            enemy.SetDestination(vector);

            cancellationTokenSource =
                new CancellationTokenSource();

            CheckPose(cancellationTokenSource.Token).Forget();
        }
    }
}
