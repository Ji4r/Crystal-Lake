using System;
using UnityEngine;

namespace MyProj
{
    public class UseProp : MonoBehaviour, ILocalOnly
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private QuickSlotInventory inventory;
        [SerializeField] private int activeSlot;
        private GameObject gameObj;


        private void OnEnable()
        {
            if (gameCamera == null)
                throw new Exception("In UseProp отсутствует ссылка на камеру");

            if (inventory == null)
            {
                Debug.LogWarning("В UseProp don't set link for QuickSlotInventory. Field - inventory = null");
                return;
            }
            inventory.ChangeActiveSlot.AddListener(ChangeActiveSlot);
        }

        private void OnDisable()
        {
            inventory.ChangeActiveSlot.RemoveListener(ChangeActiveSlot);
        }

        public void UsePropInHandle()
        {
            if (gameObj == null)
                return; 

            if (gameObj.TryGetComponent<IUseProp>(out var iUseProp))
            {
                iUseProp.Use(gameCamera);
                if (iUseProp.IsDisposable)
                    inventory.DestroyPropInHandle();
            }
        }

        private void ChangeActiveSlot(int activeSlot, GameObject gameObj)
        {
            this.activeSlot = activeSlot;
            this.gameObj = gameObj;
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }
    }
}
