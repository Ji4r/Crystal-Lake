using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public class Container : NetworkBehaviour
    {
        [SerializeField] private LootingPropsData lootingPropsData;
        [SerializeField] private byte countSlot;

        [SyncVar(hook = nameof(ChangeOpen))] private bool isOpened;

        private SyncDictionary<byte, ushort> itemWithIndexSlot = new();

        public override void OnStartServer()
        {
            lootingPropsData.Initialize();
            InitializeSlotItems();
        }

        private void ChangeOpen(bool oldValue, bool newValue)
        {
            isOpened = newValue;
        }

        [Server]
        public void OpenChest(AllPartPlayer allPartPlayer)
        {
            if (isOpened)
                return;

            TargetOpen(allPartPlayer.MyNetworkIdentity.connectionToClient, allPartPlayer);
            isOpened = true;
        }

        [TargetRpc]
        private void TargetOpen(NetworkConnection target, AllPartPlayer allPartPlayer)
        {
            Debug.Log(itemWithIndexSlot.Count);
            foreach (var pair in itemWithIndexSlot)
            {
                Debug.Log($"Slot: {pair.Key}, Item: {pair.Value}");
            }
            var viewUiLootingProps = allPartPlayer.Get<ViewUiLootingProps>();
            viewUiLootingProps.ShowGrid(countSlot, this, itemWithIndexSlot);
        }

        private void InitializeSlotItems()
        {
            if (lootingPropsData.Items.Count > countSlot)
                throw new System.Exception("Кол-во предметов больше чем слотов в данном хранилище");

            List<byte> slots = new List<byte>(countSlot);

            for (byte i = 0; i < countSlot; i++)
            {
                slots.Add(i);
            }

            for (byte i = 0; i < lootingPropsData.Items.Count; i++)
            {
                byte index = (byte)Random.Range(0, slots.Count);

                itemWithIndexSlot[slots[index]] = lootingPropsData.Items[i];

                slots.RemoveAt(index);
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdSetIsOpen(bool state)
        {
            isOpened = state;
        }
    }
}
