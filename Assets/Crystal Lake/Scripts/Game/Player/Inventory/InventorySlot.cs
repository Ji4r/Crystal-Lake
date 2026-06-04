using UnityEngine.UI;
using UnityEngine;

namespace MyProj
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private ItemScriptebleObject item;
        [SerializeField] private bool isEmpty = true;
        [SerializeField] private Image iconSlot;
        [SerializeField] private Image icon;

        public ItemScriptebleObject Item { get => item; }
        public bool IsEmpty { get => isEmpty; }
        public Image IconSlot { get => iconSlot; }
        public Image Icon { get => icon; }

        /// <summary>
        /// Устанавливает предмет в слот и обновляет его состояние. Если переданный предмет равен null,
        /// слот считается пустым, и иконка скрывается. В противном случае, иконка отображается с изображением предмета.
        /// </summary>
        /// <param name="_item"></param>
        public void SetItem(ItemScriptebleObject _item)
        {
            item = _item;
            isEmpty = _item == null ? true : false;
            SetIcon(item == null ? null : item.Icon);
        }

        private void SetIcon(Sprite _icon = null)
        {
            Debug.Log($"Icon = {_icon}");
            Debug.Log("Setting icon for slot");
            Icon.color = new Color(1, 1, 1, 1);
            if (_icon != null)
            {
                Icon.sprite = _icon;
                Icon.gameObject.SetActive(true);
            }
            else
            {
                Icon.sprite = null;
                Icon.gameObject.SetActive(false);
                Icon.color = new Color(1, 1, 1, 0);
            }
        }
    }
}
