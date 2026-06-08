using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyProj
{
    public class CardRequestIntiveLobby : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nickPlayer;
        [SerializeField] private RawImage avatarPlayer;
        [SerializeField] private Button btnAccept;
        [SerializeField] private Button btnCancel;

        private CSteamID lobbyId;

        public void SetData(string nickname, Texture2D avatar, CSteamID lobbyID)
        {
            nickPlayer.text =
                nickname;

            avatarPlayer.texture =
                avatar;

            lobbyId = lobbyID;
        }

        public void Initialize(Action<CSteamID> accept, Action cancel)
        {
            btnAccept.onClick.RemoveAllListeners();
            btnCancel.onClick.RemoveAllListeners();

            btnAccept.onClick.AddListener(() =>
            {
                accept?.Invoke(lobbyId);
            });

            btnCancel.onClick.AddListener(() =>
            {
                cancel?.Invoke();
            });
        }

        private void OnDisable()
        {
            btnAccept.onClick.RemoveAllListeners();
            btnCancel.onClick.RemoveAllListeners();
        }
    }
}