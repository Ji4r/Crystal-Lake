using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public class LootingPropsData : NetworkBehaviour
    {
        [SerializeField] private List<ItemScriptebleObject> startItems;

        public SyncList<ushort> Items { get; private set; } = new();

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
