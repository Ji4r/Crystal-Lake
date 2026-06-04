using Mirror;
using UnityEngine;

namespace MyProj
{
    public class ItemUse : Item
    {
        [Header("Однаразовый")]
        [SerializeField] private bool isDisposable;
        public bool IsDisposable => isDisposable;

        public bool ItemWasUsed { get; protected set; } // Предмет был использован


        private void Start()
        {
            ItemWasUsed = false;
        }

        public virtual void Use(Camera gameCamera, NetworkIdentity player)
        {
            Debug.Log("Use item");
        }
    }
}
