using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyProj
{
    public class CardPlayerInLobbyData : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nickPlayer;
        [SerializeField] private RawImage avatarPlayer;
        [SerializeField] private Image imageIcon;
        [SerializeField] private GameObject arrowImage;
        [SerializeField] private Sprite spriteHost;
        [SerializeField] private Sprite spriteClient;

        public bool IsBusy => isBusy;

        private bool isBusy;

        private void Start()
        {
            avatarPlayer.gameObject.SetActive(false);
            imageIcon.gameObject.SetActive(false);
            arrowImage.SetActive(false);
        }

        public void SetData(string nick, Texture2D avatar, bool isLobbyOwner)
        {
            nickPlayer.text = nick;

            if (avatar != null)
            {
                avatarPlayer.texture = avatar;
            }

            if (isLobbyOwner)
            {
                imageIcon.sprite = spriteHost;
            }
            else
            {
                imageIcon.sprite = spriteClient;
            }

            avatarPlayer.gameObject.SetActive(true);
            imageIcon.gameObject.SetActive(true);
            arrowImage.SetActive(true);

            isBusy = true;
        }

        public void ReleaseSlot()
        {
            avatarPlayer.gameObject.SetActive(false);
            imageIcon.gameObject.SetActive(false);
            arrowImage.SetActive(false);

            nickPlayer.text = string.Empty;
            avatarPlayer.texture = null;
            isBusy = false;

        }
    }
}