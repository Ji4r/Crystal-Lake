using Steamworks;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MyProj
{
    public class CardPlayerInFriendListData : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI nickPlayer;
        [SerializeField] private TextMeshProUGUI statusPlayer;
        [SerializeField] private TextMeshProUGUI textInviteButton;
        [SerializeField] private RawImage avatarPlayer;
        [SerializeField] private Button inviteButton;
        [SerializeField] private Image pointImage;

        [Header("Параметры")]
        [SerializeField] private float timeDiactivateButton = 5f;

        [Header("Цвета статусов")]
        [SerializeField] private Color onlineColor = Color.blue;
        [SerializeField] private Color myGameColor = Color.green;
        [SerializeField] private Color otherGameColor = new Color(0f, 0.5f, 0f);
        [SerializeField] private Color offlineColor = Color.gray;

        private FriendData currentFriendData;
        private LobbyManager lobbyManager;
        private Coroutine diactivateButtonCoroutine;

        [Inject]
        public void Construct(LobbyManager lobbyManager)
        {
            this.lobbyManager = lobbyManager;

            inviteButton.onClick.RemoveAllListeners();

            inviteButton.onClick.AddListener(() =>
            {
                if (currentFriendData == null)
                {
                    Debug.LogWarning(
                        "Invite failed: currentFriendData is null"
                    );

                    return;
                }

                lobbyManager.InviteFriend(
                    currentFriendData.SteamId
                );

                diactivateButtonCoroutine = StartCoroutine(
                    DiactivateButton()
                );
            });
        }

        private void OnDisable()
        {
            if (diactivateButtonCoroutine != null)
            {
                StopCoroutine(diactivateButtonCoroutine);
                diactivateButtonCoroutine = null;
            }
        }

        public void SetData(FriendData friendData)
        {
            if (friendData == null)
                return;

            currentFriendData = friendData;

            nickPlayer.text =
                friendData.Nickname;

            statusPlayer.text =
                GetStatusText(friendData);

            statusPlayer.color =
                GetStatusColor(friendData);

            pointImage.color =
                GetStatusColor(friendData);

            if (friendData.Avatar != null)
            {
                avatarPlayer.texture =
                    friendData.Avatar;
            }
        }

        private string GetStatusText(FriendData friendData)
        {
            if (friendData.IsInGame)
            {
                if (friendData.IsPlayingMyGame)
                {
                    return "В Crystal Lake";
                }

                return "В другой игре";
            }

            switch (friendData.Status)
            {
                case EPersonaState.k_EPersonaStateOffline:
                    return "Не в сети";

                case EPersonaState.k_EPersonaStateOnline:
                    return "В сети";

                case EPersonaState.k_EPersonaStateBusy:
                    return "Не беспокоить";

                case EPersonaState.k_EPersonaStateAway:
                    return "Отошел";

                case EPersonaState.k_EPersonaStateSnooze:
                    return "AFK";

                case EPersonaState.k_EPersonaStateLookingToTrade:
                    return "Хочет обменяться";

                case EPersonaState.k_EPersonaStateLookingToPlay:
                    return "Ищет игру";

                default:
                    return "Неизвестно";
            }
        }

        private Color GetStatusColor(FriendData friendData)
        {
            if (friendData.IsInGame)
            {
                if (friendData.IsPlayingMyGame)
                {
                    return myGameColor;
                }

                return otherGameColor;
            }

            if (friendData.Status ==
                EPersonaState.k_EPersonaStateOnline)
            {
                return onlineColor;
            }

            return offlineColor;
        }

        private IEnumerator DiactivateButton()
        {
            inviteButton.interactable = false;
            textInviteButton.text = "Отправлено";
            yield return new WaitForSeconds(timeDiactivateButton);
            inviteButton.interactable = true;
            textInviteButton.text = "Пригласить";
            diactivateButtonCoroutine = null;
        }
    }
}