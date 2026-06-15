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

        private void Start()
        {
            healthManager = allPartPlayer.Get<HealthManager>();
            spectatorData = allPartPlayer.Get<SpectatorData>();
            mouseLook = allPartPlayer.Get<MouseLook>();

            healthManager.OnDeathClient += HandleDeath;
        }

        private void OnDisable()
        {
            healthManager.OnDeathClient -= HandleDeath;
        }

        private void HandleDeath()
        {
            spectatorData.BtnNextPlayer(SpectateNextPlayer);
            spectatorData.BtnBackPlayer(SpectateBackToPlayer);
            currentSpectatingIndex = -1;
            playersInGame = GetAllPlayerInGame();

            if (playersInGame.Count <= 0)
            {
                // Завершать игру, если нет игроков
                Debug.LogError("Нету игроков в катке, катка оконченна");
                return;
            }

            SetSpectatorMode();
        }

        private void SetSpectatorMode()
        {
            foreach (var part in allPartPlayer.GetAll<IPartPlayer>())
            {
                if (part is CharacterSpectetor)
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
            if (!isLocalPlayer)
                return;
            Debug.Log("LocalPlayer");

            playersInGame = GetAllPlayerInGame();

            if (playersInGame.Count == 0)
                return;

            Debug.Log("БОльше 0");

            currentSpectatingIndex = (sbyte)((currentSpectatingIndex + 1) % playersInGame.Count);

            spectatorData.SetTarget(playersInGame[currentSpectatingIndex]);
            Debug.Log("Обновил таргет 0");
        }

        public void SpectateBackToPlayer()
        {
            if (!isLocalPlayer)
                return;

            playersInGame = GetAllPlayerInGame();

            if (playersInGame.Count == 0)
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

        private List<AllPartPlayer> GetAllPlayerInGame()
        {
            var players = new List<AllPartPlayer>();

            foreach (var player in AllPartPlayer.Players)
            {
                if (player == allPartPlayer)
                    continue;

                var health = player.Get<HealthManager>();

                if (health.CurrentHealth <= 0)
                    continue;

                players.Add(player);
            }

            return players;
        }
    }
}