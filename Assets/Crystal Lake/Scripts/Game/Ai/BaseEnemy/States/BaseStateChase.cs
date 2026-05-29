using UnityEngine;

namespace MyProj
{
    public class BaseStateChase : FsmAIEnemyState
    {
        private float killDistance;
        private Transform currentTraget;

        public BaseStateChase(BaseEnemy enemy, float killDistance) : base(enemy)
        {
            this.killDistance = killDistance;
        }

        public override void EnterState() 
        {
            currentTraget = enemy.CurrentTarget;
        }

        public override void ExitState() {}

        public override void UpdateState()
        {
            if (!enemy.TryFindTargetCached())
            {
                enemy.SetState(EnemyState.Searching);
                return;
            }

            if (enemy.CurrentTarget != currentTraget)
            {
                Vector3 dirOldTarget = enemy.CurrentTarget.position - enemy.MyTransform.position;
                Vector3 dirNewTarget = currentTraget.position - enemy.MyTransform.position;

                if (dirOldTarget.sqrMagnitude < dirNewTarget.sqrMagnitude && enemy.TryFindSpecificGoal(currentTraget))
                {
                    // Если новый таргет дальше, чем старый, и при этом мы видим старый таргет, то продолжаем его преследовать
                }
                else
                {
                    currentTraget = enemy.CurrentTarget;
                }
            }

            ChasePlayer();
        }

        private void ChasePlayer()
        {
            enemy.SetDestination(currentTraget.position);

            Attack();
        }

        private void Attack()
        {
            Vector3 dir = currentTraget.position - enemy.MyTransform.position;
            float sqrDistance = dir.sqrMagnitude;

            if (sqrDistance < killDistance * killDistance)
            {
                enemy.StopDestination();
                Debug.Log("Kill");
            }
        }
    }
}
