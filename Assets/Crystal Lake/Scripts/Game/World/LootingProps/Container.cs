using Mirror;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace MyProj
{
    public class Container : NetworkBehaviour
    {
        [SerializeField] private byte countSlot;

        [SyncVar(hook = nameof(ChangeOpen))] private bool isOpened;

        public byte CountSlot => countSlot; 

        private readonly SyncList<SlotContainer> itemWithIndexSlot = new();


        public override void OnStartServer()
        {
            InitListObjects();
        }

        [Command(requiresAuthority = false)]
        public void CmdSetIsOpen(bool state)
        {
            isOpened = state;
        }

        public List<SlotContainer> GetListItem()
        {
            return itemWithIndexSlot.ToList();
        }

        [Server]
        public void OpenChest(AllPartPlayer allPartPlayer)
        {
            if (isOpened)
                return;

            TargetOpen(allPartPlayer.MyNetworkIdentity.connectionToClient, allPartPlayer);
            isOpened = true;
        }

        [Server]
        public bool TryAddPropInContainer(ushort idProp)
        {
            if (itemWithIndexSlot == null || itemWithIndexSlot.Count != countSlot)
            {
                InitListObjects();

                if (itemWithIndexSlot == null || itemWithIndexSlot.Count != countSlot)
                {
                    Debug.LogError("Словарь null или пуст", this);
                    return false;
                }
            }

            List<byte> slots = new List<byte>(countSlot);

            for (byte i = 0; i < itemWithIndexSlot.Count; i++)
            {
                if (!itemWithIndexSlot[i].IsFill)
                    slots.Add(i);
            }

            if (slots.Count == 0)
            {
                Debug.LogError("Пустых слотов не было найдено");
                return false;
            }

            byte idSlot = (byte)Random.Range(0, slots.Count);
            int index = slots[idSlot];

            var slot = itemWithIndexSlot[index];
            slot.SetProp(idProp);
            itemWithIndexSlot[index] = slot;

            return true;
        }

        [TargetRpc]
        private void TargetOpen(NetworkConnection target, AllPartPlayer allPartPlayer)
        {
            Debug.Log(itemWithIndexSlot.Count(x => x.IsFill == true));
            var viewUiLootingProps = allPartPlayer.Get<ViewUiLootingProps>();
            viewUiLootingProps.ShowGrid(countSlot, this, itemWithIndexSlot);
        }

        private void ChangeOpen(bool oldValue, bool newValue)
        {
            isOpened = newValue;
        }

        private void InitListObjects()
        {
            for (byte i = 0; i < countSlot; i++)
            {
                itemWithIndexSlot.Add(new SlotContainer(false));
            }
        }
    }
}
