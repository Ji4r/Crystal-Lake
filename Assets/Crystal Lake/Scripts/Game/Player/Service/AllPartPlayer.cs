using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public class AllPartPlayer : NetworkBehaviour
    {
        [SerializeField] private Camera characterCamera;
        [SerializeField] private GameObject localHands;
        [SerializeField] private AudioListener audioListener;
        [SerializeField] private Transform voiceChatParent;
        [SerializeField] private NetworkIdentity networkIdentity;

        public Camera CharacterCamera => characterCamera;
        public AudioListener AudioListener => audioListener;
        public Transform VoiceChatParent => voiceChatParent;

        public GameObject LocalHands => localHands;
        public NetworkIdentity MyNetworkIdentity { get => networkIdentity;}

        private Dictionary<Type, IPartPlayer> partPlayer;


        [Command]
        private void CmdPlayerReady()
        {
            EntryPointGame.Instance.AddPlayerReady(this);
        }

        private void Awake()
        {
            partPlayer = new Dictionary<Type, IPartPlayer>();
            var thisTrasform = transform;

            foreach (var part in GetComponentsInChildren<IPartPlayer>())
            {
                var type = part.GetType();
                if (partPlayer.ContainsKey(type))
                    throw new Exception($"Такой тип уже зарегистрирован - {type}");

                partPlayer.Add(type, part);
            }
        }

        public override void OnStartClient()
        {
            if (!isLocalPlayer)
            {
                foreach (var part in partPlayer.Values)
                {
                    if (part is ILocalOnly localOnly)
                    {
                        localOnly.LocalDissable();
                    }
                }
            }

            CmdPlayerReady();
        }

        public override void OnStopServer()
        {
            //base.OnStopServer();

            //var manager = FindFirstObjectByType<StatePlayersManager>();

            //if (manager != null)
            //{
            //    Debug.Log("Я вышел ");
            //    manager.RemoveDisconnectedPlayer(this);
            //}
        }

        public T Get<T>() where T : class, IPartPlayer
        {
            if (partPlayer.TryGetValue(typeof(T), out var value))
                return value as T;

            Debug.LogWarning($"Такой тип не найден - {typeof(T)}");
            return null;
        }

        public List<T> GetAll<T>() where T : class, IPartPlayer
        {
            var result = new List<T>(partPlayer.Values.Count);
            foreach (var part in partPlayer.Values)
            {
                if (part is T t)
                    result.Add(t);
            }
            return result;
        }
    }
}
