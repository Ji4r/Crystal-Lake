using Mirror;
using UnityEngine;

namespace MyProj
{
    public class MyNetworkManager : NetworkManager
    {
        [SerializeField]
        private GameObject gameplayPlayerPrefab;

        public GameObject GameplayPlayerPrefab => gameplayPlayerPrefab;

        public override void OnServerSceneChanged(string sceneName)
        {
            Debug.Log($"OnServerSceneChanged: {sceneName}");
            base.OnServerSceneChanged(sceneName);

            if (sceneName != SceneName.GAME)
            {
                Debug.Log("NOT GAME SCENE");
                return;
            }

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

                NetworkServer.ReplacePlayerForConnection(
                    conn,
                    newPlayer,
                    ReplacePlayerOptions.Destroy
                );
            }

            Debug.Log("GAME SCENE LOADED");
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Debug.Log($"OnServerAddPlayer: {conn.connectionId}");

            // Если игрок уже есть — ничего не создаём
            if (conn.identity != null)
            {
                Debug.LogWarning(
                    $"Connection {conn.connectionId} already has a player"
                );

                return;
            }

            base.OnServerAddPlayer(conn);
        }
    }
}