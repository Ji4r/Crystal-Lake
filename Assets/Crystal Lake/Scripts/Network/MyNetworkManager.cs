using Mirror;
using UnityEngine;
using Zenject;

namespace MyProj
{
    public class MyNetworkManager : NetworkManager
    {
        [SerializeField]
        private GameObject gameplayPlayerPrefab;


        [Inject] private DiContainer container;

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

                container.InjectGameObject(newPlayer);
                Debug.Log(container + " Conn!!!!!!!");

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
            Transform startPos = GetStartPosition();

            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            container.InjectGameObject(player);

            player.name =
                $"{playerPrefab.name} [connId={conn.connectionId}]";

            NetworkServer.AddPlayerForConnection(conn, player);
        }
    }
}