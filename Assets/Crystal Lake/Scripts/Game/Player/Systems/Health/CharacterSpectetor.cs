using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public class CharacterSpectetor : NetworkBehaviour, IPartPlayer
    {
        [SerializeField] private AllPartPlayer allPartPlayer;

        private HealthManager healthManager;
        private List<AllPartPlayer> playersInGame;
        private sbyte currentSpectatingIndex;
        private SpectatorData spectatorData;
        private MouseLook mouseLook;
        private PlayerState playerState;

        private void Start()
        {
            healthManager = allPartPlayer.Get<HealthManager>();
            spectatorData = allPartPlayer.Get<SpectatorData>();
            mouseLook = allPartPlayer.Get<MouseLook>();
            playerState = allPartPlayer.Get<PlayerState>();

            playerState.StateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            playerState.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(PlayerState state)
        {
            if (state.CurrentState == StatesPlayer.IsSpectator)
            {
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            currentSpectatingIndex = -1;
            playersInGame = GetAllPlayerInGame();

            if (playersInGame.Count <= 0)
            {
                // Завершать игру, если нет игроков
                Debug.LogError("Нету игроков в катке, катка оконченна");
                return;
            }

            spectatorData.SetInteractibleButton(playersInGame.Count > 1 ? true : false);
            spectatorData.BtnNextPlayer(SpectateNextPlayer);
            spectatorData.BtnBackPlayer(SpectateBackToPlayer);

            SetSpectatorMode();
        }

        private void SetSpectatorMode()
        {
            foreach (var part in allPartPlayer.GetAll<IPartPlayer>())
            {
                if (part is CharacterSpectetor || part is SpectatorData)
                    continue;

                if (part is Behaviour behaviour)
                {
                    behaviour.enabled = false;
                }
            }

            spectatorData.enabled = true;
            mouseLook.enabled = true;
            mouseLook.SetStateCursor(true);

            spectatorData.EnableSpectatorMode();

            SpectateFirstPlayer();
        }

        public void SpectateNextPlayer()
        {
            if (!CheckPlayerForSpecatate())
                return;

            currentSpectatingIndex = (sbyte)((currentSpectatingIndex + 1) % playersInGame.Count);

            spectatorData.SetTarget(playersInGame[currentSpectatingIndex]);
        }

        public void SpectateBackToPlayer()
        {
            if (!CheckPlayerForSpecatate())
                return;

            if (currentSpectatingIndex >= playersInGame.Count)
            {
                currentSpectatingIndex = 0;
            }

            currentSpectatingIndex--;

            if (currentSpectatingIndex < 0)
            {
                currentSpectatingIndex = (sbyte)(playersInGame.Count - 1);
            }

            spectatorData.SetTarget(playersInGame[currentSpectatingIndex]);
        }

        private bool CheckPlayerForSpecatate()
        {
            if (!isLocalPlayer)
                return false;

            playersInGame = GetAllPlayerInGame();

            if (playersInGame.Count == 0)
                return false;

            spectatorData.SetInteractibleButton(playersInGame.Count > 1 ? true : false);
            return true;
        }

        private void SpectateFirstPlayer()
        {
            if (!isLocalPlayer)
                return;

            if (playersInGame == null || playersInGame.Count == 0)
            {
                Debug.LogError("Нету игроков для наблюдения");
                return;
            }

            currentSpectatingIndex = 0;

            spectatorData.SetTarget(playersInGame[currentSpectatingIndex]);
        }

        public void UpdateTarget()
        {
            if (!CheckPlayerForSpecatate())
                return;

            currentSpectatingIndex = 0;

            spectatorData.SetTarget(playersInGame[currentSpectatingIndex]);
        }

        private List<AllPartPlayer> GetAllPlayerInGame()
        {
            var players = new List<AllPartPlayer>();

            foreach (var player in StatePlayersManager.Players)
            {
                if (player == allPartPlayer)
                    continue;

                if (player.Get<PlayerState>().CurrentState == StatesPlayer.IsLive)
                    players.Add(player);
            }

            return players;
        }
    }
}