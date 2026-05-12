using Mirror;
using UnityEngine;

namespace MyProj
{
    public class DoorLock : NetworkBehaviour, IInteractible
    {
        [SyncVar]
        public bool isOpen;

        [SerializeField] private KeyForDoor key;

        public void Interact(RaycastHit hit)
        {
            isOpen = !isOpen; 
        }

        public void OpenDoorLock()
        {
            isOpen = !isOpen;
            Debug.Log(isOpen);
        }
    }
}
