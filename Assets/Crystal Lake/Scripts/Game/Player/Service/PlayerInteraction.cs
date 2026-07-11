using Mirror;
using UnityEngine;

namespace MyProj
{
    public class PlayerInteraction : NetworkBehaviour, IPartPlayer
    {
        [SerializeField] private AllPartPlayer allPartPlayer;

        [Command]
        public void CmdInteract(NetworkIdentity target)
        {
            if (target.TryGetComponent(out Container container))
            {
                container.OpenChest(allPartPlayer);
            }
        }
    }
}
