using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MyProj
{
    public class MenuLobbyUI : MonoBehaviour
    {
        [Header("Players UI")]
        [SerializeField]
        private CardPlayerInLobbyData[] slotForPlayers;

        [Header("Buttons")]
        [SerializeField]
        private Button buttonCreateLobby;

        [SerializeField]
        private Button buttonStartGame;
        [SerializeField]
        private Button buttonExitlobby;

        [Inject]
        private LobbyManager lobbyManager;

        private bool isLobbyCreatedOrJoined;

        #region Unity

        private void OnEnable()
        {
            buttonCreateLobby.onClick.AddListener(CreateLobby);
            buttonStartGame.onClick.AddListener(lobbyManager.StartLobby);
            buttonExitlobby.onClick.AddListener(ExitFromLobby);

            lobbyManager.OnLobbyUpdated += RefreshUI;

            RefreshUI();
        }

        private void OnDisable()
        {
            buttonCreateLobby.onClick.RemoveListener(CreateLobby);
            buttonStartGame.onClick.RemoveListener(lobbyManager.StartLobby);
            buttonExitlobby.onClick.RemoveListener(ExitFromLobby);

            lobbyManager.OnLobbyUpdated -= RefreshUI;
        }

        #endregion

        #region Lobby

        private void CreateLobby()
        {
            if (isLobbyCreatedOrJoined)
                return;

            isLobbyCreatedOrJoined = true;

            lobbyManager.CreateLobby();
        }

        private void ExitFromLobby()
        {
            if (LobbyManager.CurrentLobbyID ==
                CSteamID.Nil)
                return;
            lobbyManager.LeaveLobby();
        }

        #endregion

        #region UI

        private void RefreshUI()
        {
            isLobbyCreatedOrJoined = LobbyManager.CurrentLobbyID != CSteamID.Nil;

            List<LobbyPlayerData> players =
                lobbyManager.GetPlayers();

            RenderPlayers(players);

            UpdateStartButton();
        }

        private void RenderPlayers(
            List<LobbyPlayerData> players
        )
        {
            ClearPlayers();

            for (int i = 0; i < players.Count; i++)
            {
                if (i >= slotForPlayers.Length)
                    break;

                LobbyPlayerData player =
                    players[i];

                slotForPlayers[i].SetData(
                    player.Nickname,
                    player.Avatar,
                    player.IsLobbyOwner
                );
            }
        }

        private void ClearPlayers()
        {
            foreach (CardPlayerInLobbyData slot
                     in slotForPlayers)
            {
                slot.ReleaseSlot();
            }
        }

        private void UpdateStartButton()
        {
            if (LobbyManager.CurrentLobbyID ==
                CSteamID.Nil)
            {
                buttonStartGame.gameObject.SetActive(
                    false
                );

                return;
            }

            bool isOwner =
                SteamMatchmaking.GetLobbyOwner(
                    LobbyManager.CurrentLobbyID
                )
                == SteamUser.GetSteamID();

            buttonStartGame.gameObject.SetActive(
                isOwner
            );
        }

        #endregion
    }
}