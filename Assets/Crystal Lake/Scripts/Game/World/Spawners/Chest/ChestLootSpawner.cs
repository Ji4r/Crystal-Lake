using Cysharp.Threading.Tasks;
using Mirror;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace MyProj
{
    public class ChestLootSpawner : NetworkBehaviour
    {
        [Title("Настройки спавнера")]
        [SerializeField] private ItemForLootingChestData itemForLooting;
        [SerializeField] private AllChest allChest;

        private byte percentageChestsToFill;
        private RangeByte chestFillPercentage;


        public async UniTask<bool> StartSpawn(byte percentageChestsToFill, RangeByte chestFillPercentage)
        {
            Debug.Log("StartSpawnLoot========");
            ApplyDifferent(percentageChestsToFill, chestFillPercentage);

            await ConfigurateProp();
            return true;
        }

        private void ApplyDifferent(byte percentageChestsToFill, RangeByte chestFillPercentage)
        {
            this.percentageChestsToFill = percentageChestsToFill;
            this.chestFillPercentage = chestFillPercentage;
        }

        private async UniTask ConfigurateProp()
        {
            itemForLooting.Initialize();
            var chests = GetAvalibleChest();
            Debug.Log("Chest's count - " + chests.Count);
            FillChest(chests, itemForLooting);
            await UniTask.CompletedTask;
        }

        private List<Chest> GetAvalibleChest()
        {
            List<Chest> chests = allChest.GetChests();

            int chestsToFill = Mathf.RoundToInt(chests.Count * (percentageChestsToFill / 100f));

            List<Chest> available = new List<Chest>(chestsToFill);

            for (int i = 0; i < chestsToFill; i++)
            {
                var chest = chests[Random.Range(0, chests.Count)];

                available.Add(chest);
                chests.Remove(chest);
            }

            return available;
        }

        private void FillChest(List<Chest> chests, ItemForLootingChestData itemForLooting)
        {
            for (var i = 0; i < chests.Count; i++)
            {
                int countProp = Mathf.RoundToInt(chests[i].CountSlot * 
                    (Random.Range(chestFillPercentage.minValue, chestFillPercentage.minValue) / 100f));
                Debug.LogWarning("Chest - " + i + "CountProp - " + countProp);

                for (var j = 0; j < countProp; j++) 
                {
                    var prop = itemForLooting.Items[Random.Range(0, itemForLooting.Items.Count)];
                    Debug.Log("ItemId - " + prop + " items - " + itemForLooting.Items.Count);
                    if (!chests[i].TryAddPropInContainer(prop))
                    {
                        Debug.LogError("Не удолось добавть предмет");
                    }
                }
            }
        }
    }
}