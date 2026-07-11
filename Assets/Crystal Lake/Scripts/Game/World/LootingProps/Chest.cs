using UnityEngine;

namespace MyProj
{
    public class Chest : Container, IInteractible
    {
        public void Interact(RaycastHit hit, AllPartPlayer allPartPlayer)
        {
            Debug.Log("Нажал открыть сундук");
            allPartPlayer.Get<PlayerInteraction>().CmdInteract(netIdentity);
        }
    }
}
