using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace MyProj
{
    public class InventorySlot : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private ItemScriptebleObject item;
        [SerializeField] private bool isEmpty = true;
        [SerializeField] private Image iconSlot;
        [SerializeField] private Image icon;

        public Func<ushort, bool> OnClickSlot;

        public ItemScriptebleObject Item { get => item; }
        public bool IsEmpty { get => isEmpty; }
        public Image IconSlot { get => iconSlot; }
        public Image Icon { get => icon; }

        private bool isTransferToContainerIsUnderway;

        private void Start()
        {
            isTransferToContainerIsUnderway = false;
        }

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
            //Debug.Log(item == null);
            //Debug.Log(item?.NameItem);
            //Debug.Log(item?.Icon);
            //Debug.Log(item?.Prefab);
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

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Нажал на слот с помощью IPointerDownHandler - " + gameObject.name);
            Debug.Log("Лежит объект - " + item?.NameItem);

            if (isTransferToContainerIsUnderway && OnClickSlot != null)
                return;

            isTransferToContainerIsUnderway = true;

            if (OnClickSlot.Invoke(item.Id))
            {
                SetItem(null);
            }

            isTransferToContainerIsUnderway = false;
        }

    }
}
