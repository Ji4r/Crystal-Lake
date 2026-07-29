using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public class ItemForLootingChestData : NetworkBehaviour
    {
        [SerializeField] private List<ItemScriptebleObject> startItems;

        public readonly SyncList<ushort> items = new();
        public SyncList<ushort> Items => items;

        [Server]
        public void Initialize()
        {
            Items.Clear();

            foreach (var item in startItems)
                Items.Add(item.Id);
        }

        public void RemoveItemFromList(ushort item)
        {
            Items.Remove(item);
        }
    }
}
