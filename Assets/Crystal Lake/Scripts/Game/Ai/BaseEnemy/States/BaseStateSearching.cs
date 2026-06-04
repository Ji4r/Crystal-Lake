using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace MyProj
{
    public class BaseStateSearching : FsmAIEnemyState
    {
        // Если я потерял игрока из виду, то я иду на точку где видел его в последний раз, осматриваюсь головой влево и вправо,
        // смотрю если есть в близи какие то шкафы кровати место где можно спрятаться то проверяю места, если нету то возвращаюсь к потрулю
        private float distancetoPointOnPointSeaching;
        private int whatTimeIsWatchingSide;
        private CancellationTokenSource cancellationTokenSource;
        private bool isChekingPos;

        public BaseStateSearching(BaseEnemy enemy, float distancetoPointOnPointSeaching, float whatTimeIsWatchingSide) : base(enemy)
        {
            this.distancetoPointOnPointSeaching = distancetoPointOnPointSeaching;
            this.whatTimeIsWatchingSide = (int)whatTimeIsWatchingSide * 1000;
        }

        public override void EnterState()
        {
            isChekingPos = false;
            cancellationTokenSource = new CancellationTokenSource();
            CheckLastPositionPlayer(cancellationTokenSource.Token).Forget();
        }

        public override void ExitState() 
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
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
                enemy.SetState(EnemyState.CheckPosition);
                return;
            }

            if (isChekingPos == true)
                return;

            else if (state == StateVisionEnemy.None)
            {
                enemy.SetState(EnemyState.Patrol);
                return;
            }
        }

        private async UniTask CheckLastPositionPlayer(CancellationToken token)
        {
            try
            {
                isChekingPos = true;
                Vector3 predictedPosition = enemy.LastKnownPlayerPosition + enemy.LastMoveDirection * 4f;
                Debug.Log(enemy.LastKnownPlayerPosition);
                enemy.SetDestination(predictedPosition);

                await UniTask.WaitUntil(() =>
                {
                    Vector3 dir =
                        enemy.MyTransform.position -
                        enemy.LastKnownPlayerPosition;

                    return dir.sqrMagnitude <=
                           distancetoPointOnPointSeaching *
                           distancetoPointOnPointSeaching;

                }, cancellationToken: token);
                Debug.Log(enemy.MyTransform.position);
                await RotateTo(enemy.RadiusRotateHeadRight, token);

                await UniTask.Delay(whatTimeIsWatchingSide, cancellationToken: token);

                await RotateTo(enemy.RadiusRotateHeadLeft - enemy.RadiusRotateHeadLeft, token);

                await UniTask.Delay(whatTimeIsWatchingSide, cancellationToken: token);

                // Проверить шкафы кровати и тд
                await TryChekingSpot();

                foreach (var noise in enemy.SoundTrigger.MemoryEnemies)
                {
                    if (noise.ExpireTime > Time.time)
                    {
                        enemy.CheckPosition = noise.NoisePosition;
                        ExitState();
                        EnterState();
                        return;
                    }
                }

                enemy.SetState(EnemyState.Patrol);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                isChekingPos = false;
            }
        }

        private async UniTask TryChekingSpot()
        {
            Debug.Log("Проверяю шкафы кровати и тд");
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

    }
}