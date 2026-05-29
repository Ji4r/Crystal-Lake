using UnityEngine;

namespace MyProj
{
    public abstract class FsmAIEnemyState
    {
        protected BaseEnemy enemy;

        public FsmAIEnemyState(BaseEnemy enemy)
        {
            this.enemy = enemy;
        }

        public virtual void EnterState() { }
        public virtual void UpdateState() { }
        public virtual void ExitState() { }
    }
}
