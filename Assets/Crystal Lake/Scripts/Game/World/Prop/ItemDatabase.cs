using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    [CreateAssetMenu(menuName = "SO/Items/Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<ItemScriptebleObject> items;

        private Dictionary<ushort, ItemScriptebleObject> cache;

        public void Initialize()
        {
            cache = new Dictionary<ushort, ItemScriptebleObject>();

            foreach (var item in items)
                cache[item.Id] = item;
        }

        public ItemScriptebleObject Get(ushort id)
        {
            cache.TryGetValue(id, out var item);
            return item;
        }
    }
}