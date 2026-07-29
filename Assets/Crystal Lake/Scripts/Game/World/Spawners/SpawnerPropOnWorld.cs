using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;

namespace MyProj
{
    public class SpawnerPropOnWorld : NetworkBehaviour
    {
        [SerializeField] private ItemFromSpawn items;

        [Header("Точки спавна")]
        [SerializeField] private SpawnerByPoints spawnerByPoints;

        private byte maxPercentage;
        private byte percentageOfSpawnHigh;
        private byte percentageOfBatterySpawn; 
        private byte percentageOfSpawnAuxiliaryItems;
        private byte totalPercentage;

        public async UniTask<bool> StartSpawn(byte maxPercentage, byte percentageOfSpawnHigh,
            byte percentageOfBatterySpawn, byte percentageOfSpawnAuxiliaryItems)
        {
            await items.InitializeLists();

            if (!ApplyDifferent(maxPercentage, percentageOfSpawnHigh,
                percentageOfBatterySpawn,
                percentageOfSpawnAuxiliaryItems))
                throw new System.Exception("не удолось приминить настройки");

            await ConfigurateProp();
            return true;
        }

        private bool ApplyDifferent(byte maxPercentage, byte percentageOfSpawnHigh, 
            byte percentageOfBatterySpawn, byte percentageOfSpawnAuxiliaryItems)
        {
            this.maxPercentage = maxPercentage;
            this.percentageOfSpawnHigh = percentageOfSpawnHigh;
            this.percentageOfBatterySpawn = percentageOfBatterySpawn;
            this.percentageOfSpawnAuxiliaryItems = percentageOfSpawnAuxiliaryItems;

            totalPercentage = (byte)(percentageOfBatterySpawn + percentageOfSpawnAuxiliaryItems + percentageOfSpawnHigh);
            if (totalPercentage > maxPercentage) 
            {
                Debug.LogError($"Сумма процентов спавна предметов - {totalPercentage} больше чем максимальный порог {maxPercentage}");
                return false;
            }
            return true;
        }

        private async UniTask ConfigurateProp()
        {
            int countPointSpawn = spawnerByPoints.GetCountFreePoint();

            int maxSpawnCount = Mathf.FloorToInt(countPointSpawn * ((float)totalPercentage / maxPercentage));

            int healCount = Mathf.RoundToInt(maxSpawnCount * ((float)percentageOfSpawnHigh / totalPercentage));
            int batteryCount = Mathf.RoundToInt(maxSpawnCount * ((float)percentageOfBatterySpawn / totalPercentage));
            int auxiliaryCount = Mathf.RoundToInt(maxSpawnCount * ((float)percentageOfSpawnAuxiliaryItems / totalPercentage));

            await UniTask.WhenAll(
                SpawnProp(items.healProp, healCount),
                SpawnProp(items.batteryProp, batteryCount),
                SpawnAuxiliaryItems(auxiliaryCount)
            );
        }

        private async UniTask SpawnProp(Item[] listItem, int countSpawnProp)
        {
            for (int i = 0; i < countSpawnProp; i++)
            {
                var trans = spawnerByPoints.GetFreeRandomPos();

                var item = Instantiate(
                    listItem[Random.Range(0, listItem.Length)],
                    trans.position,
                    Quaternion.identity);

                NetworkServer.Spawn(item.gameObject);


                if (i > 0 && i % 5 == 0)
                    await UniTask.Yield();
            }
        }

        private async UniTask SpawnAuxiliaryItems(int countSpawnProp)
        {
            for (int i = 0; i < countSpawnProp; i++)
            {
                var trans = spawnerByPoints.GetFreeRandomPos();

                var item = Instantiate(
                    items.GetRandomProp(),
                    trans.position,
                    Quaternion.identity);

                NetworkServer.Spawn(item.gameObject);

                if (i > 0 && i % 5 == 0)
                    await UniTask.Yield();
            }
        }
    }
}
