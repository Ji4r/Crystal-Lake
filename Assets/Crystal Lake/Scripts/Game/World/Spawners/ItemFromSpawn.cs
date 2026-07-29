using UnityEngine;
using System.Collections.Generic;
using TriInspector;
using Cysharp.Threading.Tasks;

namespace MyProj
{
    [System.Serializable]
    public class ItemFromSpawnData
    {
        public Item item;
        public byte chance;
    }

    public class ItemFromSpawn : MonoBehaviour
    {
        [Header("Объекты для спавна")]
        public Item[] healProp;
        public Item[] batteryProp;
        [SerializeField] private ItemFromSpawnData[] itemFromSpawnData;

#if UNITY_EDITOR
        [SerializeField, ReadOnly] private int summPercentage;
#endif
        [Tooltip("Максимальный процент"), SerializeField, Range(1, 255)] private byte maxSpawnPercentageauxiliaryItems;

        private Dictionary<Item, byte> allItems;
        private bool isException;

#if UNITY_EDITOR
        private void OnValidate()
        {
            summPercentage = 0;

            foreach (var item in itemFromSpawnData) 
            {
                summPercentage += item.chance;
            }

            if (summPercentage > maxSpawnPercentageauxiliaryItems)
            {
                Debug.LogError("Сумма процентов больше максимальной", this);
            }
        }
#endif

        public async UniTask InitializeLists()
        {
            isException = !IsCalculatingSpawnPercentageItem();

            if (isException)
                throw new System.Exception("isException = true, не возможно продолжить работу");

            allItems = new Dictionary<Item, byte>(itemFromSpawnData.Length);
            for (int i = 0; i < itemFromSpawnData.Length; i++) 
            {

                if (i == 0)
                {
                    allItems[itemFromSpawnData[i].item] = itemFromSpawnData[i].chance;
                }
                else
                {
                    allItems[itemFromSpawnData[i].item] = (byte)(itemFromSpawnData[i - 1].chance + itemFromSpawnData[i].chance);
                }
            }

            await UniTask.CompletedTask;
        }

        public Item GetRandomProp()
        {
            if (isException)
                throw new System.Exception("isException = true, не возможно продолжить работу");

            byte percentageDropped = (byte)Random.Range(1, maxSpawnPercentageauxiliaryItems + 1);

            byte lowerChance = 0;
            byte highChance = 0;

            for (int i = 0; i < allItems.Count; i++) 
            {
                if (!allItems.TryGetValue(itemFromSpawnData[i].item, out highChance))
                    continue;

                if (i != 0)
                    if (!allItems.TryGetValue(itemFromSpawnData[i - 1].item, out lowerChance))
                        continue;

                if (highChance >= percentageDropped && lowerChance <= percentageDropped)
                    return itemFromSpawnData[i].item;
            }

            throw new System.Exception("Не удолось найти предмет с таким шансом");
        }

        public ItemFromSpawnData[] GetListItem()
        {
            return itemFromSpawnData;
        }

        private bool IsCalculatingSpawnPercentageItem()
        {
            int sumPercentage = 0;

            foreach (var item in itemFromSpawnData)
            {
                sumPercentage += item.chance;
            }

            if (sumPercentage == maxSpawnPercentageauxiliaryItems)
            {
                return true;
            }
            else
            {
                Debug.LogError("Сумма процентов не равна максимальной, для спавна спец. предметов, увеличьте границы ил уменьшите шансы");
                return false;
            }
        }
    }
}
