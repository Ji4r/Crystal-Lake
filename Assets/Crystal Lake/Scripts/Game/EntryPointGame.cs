using Cysharp.Threading.Tasks;
using Mirror;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;


namespace MyProj
{
    public class EntryPointGame : NetworkBehaviour
    {
        public static EntryPointGame Instance { get; private set; }

        [Title("Spawners")]
        [SerializeField] private SpawnerPropOnWorld spawnerPropOnWorld;
        [SerializeField] private ChestLootSpawner chestLootSpawner;

        [Title("Systems")]
        [SerializeField] private StatePlayersManager statePlayersManager;

        private DifficultyGameData difficultyGameData;
        private readonly HashSet<AllPartPlayer> players = new();
        private bool gameIsStart = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }


        [Server]
        public void AddPlayerReady(AllPartPlayer statePlayer) // Собираем созданных игроков
        {
            if (gameIsStart == true)
                return;

            players.Add(statePlayer);

            CheckCanStart();
        }

        [Server]
        public void CheckCanStart() //Если все игроки собраны то стартуем каточку
        {
            if (gameIsStart)
                return;

            if (players.Count != NetworkServer.connections.Count)
                return;

            gameIsStart = true;
            StartInitializeServer().Forget();
        }


        [Server]
        private async UniTask StartInitializeServer() //Инициализируем карту лутам и прочие системки
        {
            Debug.Log("===Scene is start configurate===");
            LoadConfigGame();

            var spawnOnWorld = spawnerPropOnWorld.StartSpawn(
                    difficultyGameData.MaxPercentageOfSpawn,
                    difficultyGameData.PercentageOfSpawnHigh,
                    difficultyGameData.PercentageOfBatterySpawn,
                    difficultyGameData.PercentageOfSpawnAuxiliaryItems
                );

            var fillContainer = chestLootSpawner.StartSpawn(
                    difficultyGameData.PercentageChestsToFill,
                    difficultyGameData.ChestFillPercentage
                );

            statePlayersManager.StartInit(players);

            await spawnOnWorld;
            await fillContainer;

            ConfigurateSceneFinished();
        }


        public override void OnStartClient()
        {
            //Включаем ui пока не загрузится всё или кат сцену
            //
        }


        [Server]
        public void RemoveDisconnectedPlayer(AllPartPlayer statePlayer)
        {
            players.Remove(statePlayer);
        }


        private void ConfigurateSceneFinished()
        {
            Debug.Log("===Scene is reade===");
            //Выключаем ui пока не загрузится всё или кат сцену
            //
        }

        private void LoadConfigGame()
        {
            difficultyGameData = DifficultyGame.Instance.Current;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
