using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public class StatePlayersManager : NetworkBehaviour
    {
        public static List<AllPartPlayer> Players { get; private set; } = new();


        private void OnDisable()
        {
            if (isServer)
            {
                foreach (var part in Players)
                {
                    part.Get<PlayerState>().StateChanged -= ChangeStatePlayer;
                    part.Get<HealthManager>().OnDeathServer -= DeadPlayer;
                }
            }
        }

        [Server]
        public void StartInit(HashSet<AllPartPlayer> allParts)
        {
            Players.Clear();
            Players.AddRange(allParts);

            foreach(var part in Players)
            {
                part.Get<PlayerState>().StateChanged += ChangeStatePlayer;
                part.Get<HealthManager>().OnDeathServer += DeadPlayer;
            }
        }


        [Server]
        public void RemoveDisconnectedPlayer(AllPartPlayer player)
        {
            Debug.Log($"Игрок {player.name} вышел");
            player.Get<PlayerState>().StateChanged -= ChangeStatePlayer;
            player.Get<HealthManager>().OnDeathServer -= DeadPlayer;
            Players.Remove(player);
        }

        [Server]
        public void DeadPlayer(AllPartPlayer partPlayer)
        {
            partPlayer.Get<PlayerState>().SetState(StatesPlayer.IsSpectator);
        }

        [Server]
        private void ChangeStatePlayer(PlayerState statesPlayer)
        {
            if (statesPlayer.CurrentState == StatesPlayer.IsSpectator)
            {

                // Переход в режим спектатора
            }
        }
    }
}
