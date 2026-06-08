using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public class AllPartPlayer : NetworkBehaviour
    {
        [SerializeField] private Camera characterCamera;
        [SerializeField] private Transform voiceChatParent;
        public Camera CharacterCamera => characterCamera;
        public Transform VoiceChatParent => voiceChatParent;

        private Dictionary<Type, IPartPlayer> partPlayer;
        
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
            if (isLocalPlayer) return;

            foreach (var part in partPlayer.Values)
            {
                if (part is ILocalOnly localOnly)
                {
                    localOnly.LocalDissable();
                }
            }
        }

        public T Get<T>() where T : class, IPartPlayer
        {
            if (partPlayer.TryGetValue(typeof(T), out var value))
                return value as T;

            Debug.LogWarning($"Такой тип не найден - {typeof(T)}");
            return null;
        }
    }
}
