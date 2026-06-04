using Mirror;
using UnityEngine;

namespace MyProj
{
    public class InventoryData : NetworkBehaviour, IPartPlayer
    {
        public const uint EmptySlot = 0;
        public readonly SyncList<uint> Items = new();

        public override void OnStartServer()
        {
            if (Items.Count == 0)
            {
                for (int i = 0; i < 4; i++)
                    Items.Add(0);
            }
        }

        public int TryAddItem(uint itemNetId)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == EmptySlot)
                {
                    Items[i] = itemNetId;
                    Debug.Log($"Added item {itemNetId} to slot {i}");
                    return i;
                }
            }

            return -1;
        }

        public uint RemoveItem(int slot)
        {
            if (slot < 0 || slot >= Items.Count)
                return EmptySlot;

            uint itemNetId = Items[slot];

            Items[slot] = EmptySlot;

            Debug.Log($"Removed item {itemNetId} from slot {slot}");

            return itemNetId;
        }

        public bool IsFull()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == EmptySlot)
                    return false;
            }

            return true;
        }
    }
}
