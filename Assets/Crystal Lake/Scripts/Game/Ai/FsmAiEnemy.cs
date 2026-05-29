using System;
using System.Collections.Generic;

namespace MyProj
{
    public class FsmAiEnemy
    {
        public FsmAIEnemyState StateCurrent { get; private set; }

        private Dictionary<Type, FsmAIEnemyState> stateDictionary;

        public FsmAiEnemy()
        {
            stateDictionary = new Dictionary<Type, FsmAIEnemyState>();
        }

        public void AddState(FsmAIEnemyState state)
        {
            stateDictionary.Add(state.GetType(), state);
        }

        public void ChangeState<T>() where T : FsmAIEnemyState
        {
            var type = typeof(T);

            if (StateCurrent != null && type == StateCurrent.GetType()) 
                return;

            if (stateDictionary.TryGetValue(type, out FsmAIEnemyState newState))
            {
                StateCurrent?.ExitState();

                StateCurrent = newState;

                StateCurrent?.EnterState();
            }
        }

        public void UpdateState()
        {
            StateCurrent?.UpdateState();
        }
    }
}
