using System.Collections.Generic;
using Steamworks;
using TMPro;
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
        [SerializeField] private Button buttonCreateLobby;
        [SerializeField] private Button buttonStartGame;
        [SerializeField] private Button buttonExitlobby;

        [Header("Текст")]
        [SerializeField] private TextMeshProUGUI countPlayerInLobby;
        [SerializeField] private string textCountPlayerInLobbyFormat = "Players";

        [Header("Invites")]
        [SerializeField] private Transform inviteContainer;
        [SerializeField] private Image scrollbarBackground;
        [SerializeField] private Image scrollbarHandle;
        [SerializeField] private CardRequestIntiveLobby invitePrefab;

        [Inject] private LobbyManager lobbyManager;
        [Inject] private MyNetworkManager myNetworkManager;

        private bool isLobbyCreatedOrJoined;
        private Color scrollbarlBackgroundOriginalColor;
        private Color scrollbarVerticalOriginalColor;
        private Color inviseColor;


        #region Unity

        private void Start()
        {
            scrollbarlBackgroundOriginalColor = scrollbarBackground.color;
            scrollbarVerticalOriginalColor = scrollbarHandle.color;
            inviseColor = new Color(0, 0, 0, 0);

            scrollbarBackground.color = inviseColor;
            scrollbarHandle.color = inviseColor;
        }

        private void OnEnable()
        {
            lobbyManager.OnLobbyInviteReceived += CreateInviteCard;
            buttonCreateLobby.onClick.AddListener(CreateLobby);
            buttonStartGame.onClick.AddListener(lobbyManager.StartLobby);
            buttonExitlobby.onClick.AddListener(ExitFromLobby);

            lobbyManager.OnLobbyUpdated += RefreshUI;

            RefreshUI();
        }

        private void OnDisable()
        {
            lobbyManager.OnLobbyInviteReceived -= CreateInviteCard;
            buttonCreateLobby.onClick.RemoveListener(CreateLobby);
            buttonStartGame.onClick.RemoveListener(lobbyManager.StartLobby);
            buttonExitlobby.onClick.RemoveListener(ExitFromLobby);

            lobbyManager.OnLobbyUpdated -= RefreshUI;
        }

        #endregion

        private void CreateInviteCard(CSteamID inviterId,CSteamID lobbyId)
        {
            scrollbarBackground.color = scrollbarlBackgroundOriginalColor;
            scrollbarHandle.color = scrollbarVerticalOriginalColor;

            CardRequestIntiveLobby card = Instantiate(invitePrefab, inviteContainer);

            string nickname = SteamFriends.GetFriendPersonaName(inviterId);

            Texture2D avatar = SteamExtenshionalTool.GetSteamAvatar(inviterId);

            card.SetData(nickname, avatar, lobbyId);

            card.Initialize((lobbyId) => 
                {
                    SteamMatchmaking.JoinLobby(lobbyId);
                    Destroy(card.gameObject);
                },

                () => 
                {
                    Destroy(card.gameObject);
                    UpdateInviteScrollbar();
                }
            );
        }

        private void UpdateInviteScrollbar()
        {
            bool hasInvites = inviteContainer.childCount > 0;

            scrollbarBackground.color = hasInvites ? scrollbarlBackgroundOriginalColor : inviseColor;
            scrollbarHandle.color = hasInvites ? scrollbarVerticalOriginalColor : inviseColor;
        }

        #region Lobby

        private void CreateLobby()
        {
            if (isLobbyCreatedOrJoined)
                return;

            isLobbyCreatedOrJoined = true;

            lobbyManager.CreateLobby();
        }

        private void UpdateCountPlayerInLobbyText(int count)
        {
            countPlayerInLobby.text = $"{count}/{myNetworkManager.maxConnections} " + textCountPlayerInLobbyFormat;
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

            UpdateCountPlayerInLobbyText(players.Count);

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