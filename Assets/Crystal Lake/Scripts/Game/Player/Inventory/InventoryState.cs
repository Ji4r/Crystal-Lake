using Mirror;
using System;
using UnityEngine;

namespace MyProj
{
    public class InventoryState : NetworkBehaviour, IPartPlayer
    {
        [SyncVar] public int ActiveSlot;
        [SyncVar(hook = nameof(OnActiveItemChanged))]
        public uint ActiveItemNetId;

        public event Action<uint, uint> OnActiveItemChangedExternal;

        private void OnActiveItemChanged(uint oldValue, uint newValue)
        {
            Debug.Log(
    $"[{name}] ActiveItem changed {oldValue} -> {newValue}");

            OnActiveItemChangedExternal?.Invoke(oldValue, newValue);
        }
    }
}