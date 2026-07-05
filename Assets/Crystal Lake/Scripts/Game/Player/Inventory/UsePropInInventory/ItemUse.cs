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


        protected virtual void Start()
        {
            ItemWasUsed = false;
        }

        public virtual void Use(Camera gameCamera, AllPartPlayer player)
        {
            Debug.Log("Use item");
            throw new System.NotImplementedException();
        }
    }
}
