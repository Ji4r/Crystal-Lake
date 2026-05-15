using UnityEngine;
using System.Linq;

namespace MyProj
{
    [System.Serializable]
    public class SpawnPointData
    {
        public Transform spawnPoint;
        [Tooltip("Точка под один предмет")]
        public bool pointForOneItem = false;
        public bool isFreePoint = true;
    }

    public class SpawnerByPoints : MonoBehaviour
    {
        [SerializeField] private SpawnPointData[] spawnPoints;

        /// <summary>
        /// Возвращает случайный Transform точки появления из массива spawnPoints. Игнорирует состояние точек (занята или свободна) и не изменяет его. Выбрасывает исключение, если массив spawnPoints null или пустой.
        /// </summary>
        /// <remarks>Выбрасывает исключение при null или пустом массиве spawnPoints.</remarks>
        /// <returns>Transform выбранной точки появления.</returns>
        public Transform GetRandomPos()
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)].spawnPoint;
        }

        /// <summary>
        /// Возвращает Transform случайно выбранной свободной точки спавна.
        /// </summary>
        /// <remarks>Если выбранная точка предназначена для одного предмета (pointForOneItem), отмечает её
        /// как занятую (isFreePoint = false). При отсутствии свободных точек записывает сообщение об ошибке и
        /// возвращает null.</remarks>
        /// <returns>Transform выбранной точки спавна или null, если свободных точек нет.</returns>
        public Transform GetFreeRandomPos()
        {
            var freePoints = spawnPoints.Where(x => x.isFreePoint).ToArray();
            if (freePoints.Length == 0)
            {
                Debug.LogError("Нет свободных точек для спавна");
                return null;
            }

            var point = freePoints[Random.Range(0, freePoints.Length)];
            if (point.pointForOneItem)
                point.isFreePoint = false;

            return point.spawnPoint;
        }

        /// <summary>
        /// Освобождает указанный Transform точки спавна, помечая её как свободную для размещения предметов, если точка
        /// предназначена для одного предмета.
        /// </summary>
        /// <remarks>Если совпадающая точка не найдена, изменений не вносится. Поиск прекращается после
        /// первого совпадения.</remarks>
        /// <param name="point">Transform точки спавна для освобождения.</param>
        public void ReleasePoint(Transform point)
        {
            foreach (var spawnPoint in spawnPoints)
            {
                if (spawnPoint.spawnPoint == point)
                {
                    for (int i = spawnPoint.spawnPoint.childCount - 1; i >= 0; i--)
                    {
                        Destroy(spawnPoint.spawnPoint.GetChild(i).gameObject);
                    }

                    if (spawnPoint.pointForOneItem)
                        spawnPoint.isFreePoint = true;

                    return;
                }
            }
        }
    }
}
