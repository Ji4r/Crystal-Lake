using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MyProj
{
    public class FriendListManager : MonoBehaviour
    {
        [SerializeField] private Button buttonRefreshFriends;
        [SerializeField] private Button buttonOpenFriendsList;
        [SerializeField] private TextMeshProUGUI textFriendCount;

        [Header("Настройки карточки")]
        [SerializeField] private CardPlayerInFriendListData cardPrefab;
        [SerializeField] private Transform contentParent;

        [HideInInspector] public List<FriendData> Friends = new();

        [Inject] private DiContainer container;

        private void OnEnable()
        {
            buttonRefreshFriends.onClick.AddListener(LoadFriends);
            buttonOpenFriendsList.onClick.AddListener(LoadFriends);
        }

        private void OnDisable()
        {
            buttonRefreshFriends.onClick.RemoveListener(LoadFriends);
            buttonOpenFriendsList.onClick.RemoveListener(LoadFriends);
        }

        public void LoadFriends()
        {
            Friends.Clear();

            int friendCount =
                SteamFriends.GetFriendCount(
                    EFriendFlags.k_EFriendFlagImmediate
                );

            for (int i = 0; i < friendCount; i++)
            {
                CSteamID friendId =
                    SteamFriends.GetFriendByIndex(
                        i,
                        EFriendFlags.k_EFriendFlagImmediate
                    );

                string nickname =
                    SteamFriends.GetFriendPersonaName(friendId);

                EPersonaState state =
                    SteamFriends.GetFriendPersonaState(friendId);

                Texture2D avatar =
                    SteamExtenshionalTool.GetSteamAvatar(friendId);

                FriendGameInfo_t gameInfo;

                bool isInGame =
                    SteamFriends.GetFriendGamePlayed(
                        friendId,
                        out gameInfo
                    );

                bool isPlayingMyGame = false;

                if (isInGame)
                {
                    AppId_t currentGameId =
                        SteamUtils.GetAppID();

                    if (gameInfo.m_gameID.AppID() == currentGameId)
                    {
                        isPlayingMyGame = true;
                    }
                }

                FriendData friendData = new FriendData
                {
                    SteamId = friendId,
                    Nickname = nickname,
                    Status = state,
                    Avatar = avatar,
                    IsInGame = isInGame,
                    IsPlayingMyGame = isPlayingMyGame
                };

                Friends.Add(friendData);
            }

            CreateCardFriends();

            textFriendCount.text = Friends.Count.ToString();
        }

        private void CreateCardFriends()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            Friends.Sort((a, b) =>
            {
                int aPriority = GetFriendPriority(a);
                int bPriority = GetFriendPriority(b);

                return aPriority.CompareTo(bPriority);
            });

            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var friend in Friends)
            {
                CardPlayerInFriendListData card = container.InstantiatePrefabForComponent<CardPlayerInFriendListData>
                    (
                        cardPrefab.gameObject,
                        contentParent
                    );

                card.SetData(friend);
            }
        }

        private int GetFriendPriority(FriendData friend)
        {
            // 0 = В твоей игре
            if (friend.IsPlayingMyGame)
                return 0;

            // 1 = В другой игре
            if (friend.IsInGame)
                return 1;

            // 2 = В сети
            if (friend.Status == EPersonaState.k_EPersonaStateOnline)
                return 2;

            // 3 = Остальные
            return 3;
        }
    }

    [System.Serializable]
    public class FriendData
    {
        public CSteamID SteamId;
        public string Nickname;
        public EPersonaState Status;
        public Texture2D Avatar;

        public bool IsInGame;
        public bool IsPlayingMyGame;
    }
}