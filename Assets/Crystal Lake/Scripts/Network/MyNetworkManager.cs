using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;
using Zenject;

namespace MyProj
{
    public class MyNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject gameplayPlayerPrefab;

        [Inject] private DiContainer container;
        private DiContainer sceneContainer;

        public GameObject GameplayPlayerPrefab => gameplayPlayerPrefab;
        public bool IsGameStarted { get; private set; }

        private EntryPointGame entryPointGame;
        private StatePlayersManager playersManager;


        public override void OnServerSceneChanged(string sceneName)
        {
            sceneContainer = null;
            playersManager = null;
            entryPointGame = null;

            Debug.Log($"OnServerSceneChanged: {sceneName}");
            base.OnServerSceneChanged(sceneName);

            if (sceneName != SceneName.GAME)
            {
                IsGameStarted = false;
                Debug.Log("NOT GAME SCENE");
                return;
            }

            IsGameStarted = true;

            foreach (NetworkConnectionToClient conn
                     in NetworkServer.connections.Values)
            {
                if (conn == null)
                    continue;

                if (conn.identity == null)
                    continue;

                GameObject oldPlayer =
                    conn.identity.gameObject;

                Transform startPos =
                    GetStartPosition();

                GameObject newPlayer =
                    Instantiate(
                        gameplayPlayerPrefab,
                        startPos.position,
                        startPos.rotation
                    );

                container.InjectGameObject(newPlayer);
                Debug.Log(container + " Conn!!!!!!!");

                NetworkServer.ReplacePlayerForConnection(
                    conn,
                    newPlayer,
                    ReplacePlayerOptions.Destroy
                );
            }

            sceneContainer = FindFirstObjectByType<SceneContext>().Container;

            playersManager = sceneContainer.Resolve<StatePlayersManager>();
            entryPointGame = sceneContainer.Resolve<EntryPointGame>();

            Debug.Log("GAME SCENE LOADED");
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Transform startPos = GetStartPosition();

            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            container.InjectGameObject(player);

            player.name =
                $"{playerPrefab.name} [connId={conn.connectionId}]";

            NetworkServer.AddPlayerForConnection(conn, player);
        }


        public override void OnServerDisconnect(NetworkConnectionToClient conn) // Отключение игрока
        {
            if (!IsGameStarted)
            {
                base.OnServerDisconnect(conn);
                return;
            }

            if (conn.identity != null && 
                conn.identity.TryGetComponent<AllPartPlayer>(out var player))
            {
                playersManager?.RemoveDisconnectedPlayer(player);
                entryPointGame?.RemoveDisconnectedPlayer(player);
            }

            base.OnServerDisconnect(conn);

            if (IsGameStarted)
                entryPointGame.CheckCanStart();
        }
    }
}