using Mirror;
using UnityEngine;

namespace MyProj
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Item : NetworkBehaviour, IInteractible
    {

        [SerializeField] private NetworkTransformReliable networkTransform;
        [SerializeField] private Collider itemCollider;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private MeshRenderer[] renderers;

        public ItemScriptebleObject item;

        public void SetVisible(bool value)
        {
            if (!isServer)
                return;
        }

        public void ShowVisual()
        {
            foreach (var renderer in renderers)
            {
                renderer.enabled = true;
            }

            itemCollider.enabled = true;
        }

        public void HideVisual()
        {
            foreach (var renderer in renderers)
            {
                renderer.enabled = false;
            }

            itemCollider.enabled = false;
        }

        [ClientRpc]
        public void RpcSetVisible(bool value)
        {
            rb.isKinematic = !value;

            networkTransform.enabled = value;
        }


        public void Interact(RaycastHit hit, AllPartPlayer allPartPlayer)
        {
        }
    }
}
