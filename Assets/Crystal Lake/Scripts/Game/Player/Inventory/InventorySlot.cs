using UnityEngine;
using UnityEngine.UI;

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

        public void ClearSlot()
        {
            SetItem(null);
        }

        private void SetIcon(Sprite _icon = null)
        {
            Debug.Log(item == null);
            Debug.Log(item?.NameItem);
            Debug.Log(item?.Icon);
            Debug.Log(item?.Prefab);
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
