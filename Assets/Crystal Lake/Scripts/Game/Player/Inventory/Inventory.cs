using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public class Inventory : NetworkBehaviour, IPartPlayer
    {
        public float throwForce;
        public Transform posDropItem;

        [SerializeField] private AllPartPlayer allPartPlayer;
        [SerializeField] private Transform ineventoryPanel;

        public QuickSlotInventory quiclSlotInventory;
        public List<InventorySlot> slots = new List<InventorySlot>();

        private InventoryData inventoryData;

        private void Awake()
        {
            inventoryData = allPartPlayer.Get<InventoryData>();
        }

        public override void OnStartLocalPlayer()
        {
            inventoryData.Items.OnChange += OnInventoryChanged;
            for (int i = 0; i < ineventoryPanel.childCount; i++)
            {
                if (ineventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
                {
                    slots.Add(ineventoryPanel.GetChild(i).GetComponent<InventorySlot>());
                }
            }
            RefreshUI();
        }

        public override void OnStopClient()
        {
            inventoryData.Items.OnChange -= OnInventoryChanged;
        }

        private void OnInventoryChanged(SyncList<uint>.Operation op, int itemIndex, uint item)
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                uint netId = inventoryData.Items[i];

                if (netId == 0)
                {
                    slots[i].SetItem(null);
                    continue;
                }

                if (!NetworkClient.spawned.TryGetValue(netId, out var identity))
                {
                    slots[i].SetItem(null);
                    continue;
                }

                Item item = identity.GetComponent<Item>();

                slots[i].SetItem(item.item);
            }
        }
    }
}
