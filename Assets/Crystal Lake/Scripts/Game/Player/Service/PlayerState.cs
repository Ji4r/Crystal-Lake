using Mirror;
using System;

namespace MyProj
{
    public class PlayerState : NetworkBehaviour, IPartPlayer
    {
        [SyncVar(hook = nameof(OnStateChanged))]
        private StatesPlayer currentState;

        public event Action<PlayerState> StateChanged;

        public StatesPlayer CurrentState => currentState;


        [Server]
        public void SetState(StatesPlayer state)
        {
            currentState = state;
        }

        private void OnStateChanged(StatesPlayer oldValue, StatesPlayer newValue)
        {
            StateChanged?.Invoke(this);
        }
    }
}
