using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MyProj
{
    [RequireComponent(typeof(Inventory))]
    public class QuickSlotInventory : NetworkBehaviour, IPartPlayer
    {
        [SerializeField] private AllPartPlayer allPartPlayer;
        [SerializeField] private Transform QuickInventoryPanel;

        [HideInInspector] public List<InventorySlot> slots = new List<InventorySlot>();

        private InventoryState inventoryState;
        private InventoryData inventoryData;
        private int lastActiveSlot;

        private void Awake()
        {
            inventoryState = allPartPlayer.Get<InventoryState>();
            inventoryData = allPartPlayer.Get<InventoryData>();
        }

        public override void OnStartLocalPlayer()
        {
            lastActiveSlot = -1;
            for (int i = 0; i < QuickInventoryPanel.childCount; i++)
            {
                if (QuickInventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
                {
                    slots.Add(QuickInventoryPanel.GetChild(i).GetComponent<InventorySlot>());
                }
            }

            if (slots.Count != 0)
            {
                SetActiveSlot(0);
            }
        }

        public void SetActiveSlot(int indexSlot)
        {
            CmdSetActiveSlot(indexSlot);
        }

        [Command]
        private void CmdSetActiveSlot(int indexSlot)
        {
            inventoryState.ActiveSlot = indexSlot;
            if (indexSlot >= inventoryData.Items.Count)
                return;

            inventoryState.ActiveItemNetId =
                inventoryData.Items[indexSlot];

            RpcSetActiveSlot(indexSlot);
        }

        public int GetActiveSlot()
        {
            return lastActiveSlot;
        }


        [ClientRpc]
        private void RpcSetActiveSlot(int indexSlot)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (i == indexSlot)
                    slots[i].IconSlot.color = Color.gray;
                else
                    slots[i].IconSlot.color = Color.white;
            }

            lastActiveSlot = indexSlot;
        }
    }
}