using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public class Inventory : NetworkBehaviour, IPartPlayer
    {
        [SerializeField] private Transform ineventoryPanel;
        [SerializeField] private float throwForce;
        [SerializeField] private Transform posDropItem;
        public QuickSlotInventory quiclSlotInventory;
        [SerializeField] private Camera mainCamera;

        [HideInInspector] public List<InventorySlot> slots = new List<InventorySlot>();

        private void Start()
        {
            for (int i = 0; i < ineventoryPanel.childCount; i++)
            {
                if (ineventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
                {
                    slots.Add(ineventoryPanel.GetChild(i).GetComponent<InventorySlot>());
                }
            }
        }

        public void AddItem(ItemScriptebleObject _item, GameObject objectTake)
        {
            int i = 0;

            foreach (InventorySlot slot in slots)
            {
                if (slot.IsEmpty == true)
                {
                    slot.SetItem(_item);
                    int activeSlot = quiclSlotInventory.GetActiveSlot();
                    quiclSlotInventory.SetParentFromProp(objectTake);

                    var idSlot = objectTake.AddComponent<IndifecatorSlot>();
                    idSlot.idSlot = (byte)i;

                    if (i == activeSlot)
                    {
                        quiclSlotInventory.DissablePropInHandle();
                        quiclSlotInventory.CmdSetPropInHandle(activeSlot);
                    }
                    return;
                }
                i++;
            }
        }

        public void DropItem()
        {
            int activeSlot = quiclSlotInventory.GetActiveSlot();

            if (slots[activeSlot].Item == null)
            { return; }

            if (slots[activeSlot].Item.Prefab == null)
            { return; }

            Vector3 throwDirection = (mainCamera.transform.forward + Vector3.up * 0.15f).normalized;
            var gameObj = quiclSlotInventory.GetCurrentProp();

            if (gameObj == null)
                return;

            gameObj.layer = slots[activeSlot].Item.DefaultLayer;
            var rb = gameObj.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.linearVelocity = throwDirection * throwForce;

            slots[activeSlot].SetItem(null);
            quiclSlotInventory.DropProp(activeSlot);
        }
    }
}
