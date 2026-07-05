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

        public override void OnStartServer()
        {
            items.InitializeLists();
            ApplyDifferent();
            SpawnItem();
        }

        private void ApplyDifferent()
        {
            maxPercentage = DifficultyGame.Instance.Current.MaxPercentageOfSpawn;
            percentageOfSpawnHigh = DifficultyGame.Instance.Current.PercentageOfSpawnHigh;
            percentageOfBatterySpawn = DifficultyGame.Instance.Current.PercentageOfBatterySpawn;
            percentageOfSpawnAuxiliaryItems = DifficultyGame.Instance.Current.PercentageOfSpawnAuxiliaryItems;
            totalPercentage = (byte)(percentageOfBatterySpawn + percentageOfSpawnAuxiliaryItems + percentageOfSpawnHigh);
            if (totalPercentage > maxPercentage)
            {
                Debug.LogError($"Сумма процентов спавна предметов - {totalPercentage} больше чем максимальный порог {maxPercentage}");
            }
        }

        private void SpawnItem()
        {
            int countPointSpawn = spawnerByPoints.GetCountFreePoint();

            int maxSpawnCount = Mathf.FloorToInt(countPointSpawn * ((float)totalPercentage / maxPercentage));

            int healCount = Mathf.RoundToInt(maxSpawnCount * ((float)percentageOfSpawnHigh / totalPercentage));
            int batteryCount = Mathf.RoundToInt(maxSpawnCount * ((float)percentageOfBatterySpawn / totalPercentage));
            int auxiliaryCount = Mathf.RoundToInt(maxSpawnCount * ((float)percentageOfSpawnAuxiliaryItems / totalPercentage));

            SpawnProp(items.healProp, healCount);
            SpawnProp(items.batteryProp, batteryCount);
            SpawnAuxiliaryItems(auxiliaryCount);
        }

        private void SpawnProp(Item[] listItem, int countSpawnProp)
        {
            for (int i = 0; i < countSpawnProp; i++)
            {
                var trans = spawnerByPoints.GetFreeRandomPos();

                var item = Instantiate(
                    listItem[Random.Range(0, listItem.Length)],
                    trans.position,
                    Quaternion.identity);

                NetworkServer.Spawn(item.gameObject);
            }
        }

        private void SpawnAuxiliaryItems(int countSpawnProp)
        {
            for (int i = 0; i < countSpawnProp; i++)
            {
                var trans = spawnerByPoints.GetFreeRandomPos();

                var item = Instantiate(
                    items.GetRandomProp(),
                    trans.position,
                    Quaternion.identity);

                NetworkServer.Spawn(item.gameObject);
            }
        }
    }
}
