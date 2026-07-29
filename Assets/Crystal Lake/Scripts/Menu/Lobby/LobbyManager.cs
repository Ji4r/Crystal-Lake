using Mirror;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MyProj
{
    public class LobbyPlayerData
    {
        public CSteamID SteamID;
        public string Nickname;
        public Texture2D Avatar;
        public bool IsLobbyOwner;
        public bool IsLocalPlayer;
    }

    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private DifficultyGame difficultyPrefab;
        [SerializeField] private TypeDiffecaltyGame difficultyGame;

        public static CSteamID CurrentLobbyID { get; private set; }

        public event Action OnLobbyUpdated;
        public event Action<CSteamID, CSteamID> OnLobbyInviteReceived;

        private const string HostAddressKey = "HostAddress";

        private Callback<LobbyCreated_t> lobbyCreatedCallback;
        private Callback<GameLobbyJoinRequested_t> lobbyJoinRequestedCallback;
        private Callback<LobbyEnter_t> lobbyEnterCallback;
        private Callback<LobbyChatUpdate_t> lobbyChatUpdateCallback;
        private Callback<LobbyInvite_t> lobbyInviteCallback;

        [Inject] private MyNetworkManager myNetworkManager;

        private void Awake()
        {
            lobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            lobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
            lobbyEnterCallback = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            lobbyChatUpdateCallback = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
            lobbyInviteCallback = Callback<LobbyInvite_t>.Create(OnLobbyInvite);
        }

        #region Public

        public void CreateLobby()
        {
            SteamMatchmaking.CreateLobby(
                ELobbyType.k_ELobbyTypeFriendsOnly,
                myNetworkManager.maxConnections
            );
        }

        private void OnLobbyInvite(LobbyInvite_t callback)
        {
            Debug.Log($"Invite from {callback.m_ulSteamIDUser}");

            OnLobbyInviteReceived?.Invoke(
            new CSteamID(callback.m_ulSteamIDUser),
            new CSteamID(callback.m_ulSteamIDLobby)
            );
        }

        public void StartLobby()
        {
            if (!NetworkServer.active)
                return;

            myNetworkManager.playerPrefab =
                myNetworkManager.GameplayPlayerPrefab;

            DifficultyGame game = Instantiate(difficultyPrefab);
            game.SetDifficulty(difficultyGame);

            NetworkServer.Spawn(game.gameObject);

            myNetworkManager.ServerChangeScene(SceneName.GAME);
        }

        public void LeaveLobby()
        {
            if (CurrentLobbyID == CSteamID.Nil)
                return;

            SteamMatchmaking.LeaveLobby(CurrentLobbyID);

            CurrentLobbyID = CSteamID.Nil;

            if (NetworkServer.active &&
                NetworkClient.isConnected)
            {
                myNetworkManager.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                myNetworkManager.StopClient();
            }

            OnLobbyUpdated?.Invoke();
        }

        public void InviteFriend(CSteamID friendSteamID)
        {
            if (CurrentLobbyID == CSteamID.Nil)
            {
                Debug.LogWarning("Lobby not created");
                return;
            }

            bool success =
                SteamMatchmaking.InviteUserToLobby(
                    CurrentLobbyID,
                    friendSteamID
                );

            Debug.Log(
                $"Invite send to {friendSteamID} : {success}"
            );
        }

        public List<LobbyPlayerData> GetPlayers()
        {
            List<LobbyPlayerData> players =
                new List<LobbyPlayerData>();

            if (CurrentLobbyID == CSteamID.Nil)
                return players;

            int membersCount =
                SteamMatchmaking.GetNumLobbyMembers(
                    CurrentLobbyID
                );

            CSteamID ownerID =
                SteamMatchmaking.GetLobbyOwner(
                    CurrentLobbyID
                );

            for (int i = 0; i < membersCount; i++)
            {
                CSteamID memberID =
                    SteamMatchmaking.GetLobbyMemberByIndex(
                        CurrentLobbyID,
                        i
                    );

                LobbyPlayerData player =
                    new LobbyPlayerData
                    {
                        SteamID = memberID,

                        Nickname =
                            SteamFriends.GetFriendPersonaName(
                                memberID
                            ),

                        Avatar =
                            SteamExtenshionalTool.GetSteamAvatar(
                                memberID
                            ),

                        IsLobbyOwner =
                            memberID == ownerID,

                        IsLocalPlayer =
                            memberID ==
                            SteamUser.GetSteamID()
                    };

                players.Add(player);
            }

            return players;
        }

        #endregion

        #region Steam Callbacks

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError("Failed create lobby");
                return;
            }

            CurrentLobbyID =
                new CSteamID(callback.m_ulSteamIDLobby);

            SteamMatchmaking.SetLobbyData(
                CurrentLobbyID,
                HostAddressKey,
                SteamUser.GetSteamID().ToString()
            );

            myNetworkManager.StartHost();
        }

        private void OnJoinRequest(GameLobbyJoinRequested_t callback)
        {
            if (CurrentLobbyID != CSteamID.Nil)
            {
                LeaveLobby();
            }

            SteamMatchmaking.JoinLobby(
                callback.m_steamIDLobby
            );
        }

        private void OnLobbyEntered(
            LobbyEnter_t callback)
        {
            CurrentLobbyID =
                new CSteamID(callback.m_ulSteamIDLobby);

            OnLobbyUpdated?.Invoke();

            if (NetworkServer.active ||
                NetworkClient.isConnected)
                return;

            string hostAddress =
                SteamMatchmaking.GetLobbyData(
                    CurrentLobbyID,
                    HostAddressKey
                );

            if (string.IsNullOrEmpty(hostAddress))
            {
                Debug.LogError("Host address not found");
                return;
            }

            myNetworkManager.networkAddress =
                hostAddress;

            myNetworkManager.StartClient();
        }

        private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
        {
            if ((ulong)CurrentLobbyID !=
                callback.m_ulSteamIDLobby)
                return;

            OnLobbyUpdated?.Invoke();
        }

        #endregion
    }
}