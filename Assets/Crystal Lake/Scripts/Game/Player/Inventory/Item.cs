using Mirror;
using UnityEngine;

namespace MyProj
{
    [RequireComponent(typeof(Rigidbody), typeof(NetworkTransformReliable))]
    public abstract class Item : NetworkBehaviour, IInteractible
    {
        [Header("Найстройки предмета")]
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

            itemCollider.enabled = false;
        }

        public void CmdHideVisual()
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
            if (value)
            {
                ShowVisual();
            }
            else
            {
                CmdHideVisual();
            }
        }

        [ClientRpc]
        public void RpcDropItem()
        {
            itemCollider.enabled = true;
        }


        public void Interact(RaycastHit hit, AllPartPlayer allPartPlayer)
        {
        }
    }
}
