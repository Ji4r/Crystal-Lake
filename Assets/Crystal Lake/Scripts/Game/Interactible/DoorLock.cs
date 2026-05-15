using Mirror;
using UnityEngine;

namespace MyProj
{
    public class DoorLock : NetworkBehaviour, IInteractible
    {
        [SyncVar]
        public bool isOpen;

        [SerializeField] private KeyForDoor key;

        public void Interact(RaycastHit hit, AllPartPlayer allPartPlayer)
        {
            isOpen = !isOpen; 
        }

        public virtual void OpenDoorLock()
        {
            isOpen = !isOpen;
            Debug.Log(isOpen);
        }
    }
}
