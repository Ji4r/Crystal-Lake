using Mirror;
using UnityEngine;

namespace MyProj
{
    public class UseProp : MonoBehaviour, ILocalOnly
    {
        [SerializeField] private AllPartPlayer allPartPlayer;
        [SerializeField] private NetworkIdentity thisPlayer;
        [SerializeField] private Camera gameCamera;
        
        private HandController handController;

        private void Awake()
        {
            handController = allPartPlayer.Get<HandController>();
        }

        public void UsePropInHandle()
        {
            GameObject gameObj = handController.CurrentItem;

            if (gameObj == null)
                return;

            if (gameObj.TryGetComponent<ItemUse>(out var iUseProp))
            {
                iUseProp.Use(gameCamera, thisPlayer);
            }
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }
    }
}
