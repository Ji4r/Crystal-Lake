using UnityEngine;

namespace MyProj
{
    public interface IInteractible
    {
        public void Interact(RaycastHit hit, AllPartPlayer allPartPlayer);
    }
}
