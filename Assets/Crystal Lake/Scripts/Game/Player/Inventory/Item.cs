using Mirror;
using UnityEngine;

namespace MyProj
{
    [System.Serializable]
    [RequireComponent(typeof(Rigidbody), typeof(NetworkTransformReliable))]
    public abstract class Item : NetworkBehaviour, IInteractible
    {
        [Header("Найстройки предмета")]
        [SerializeField] private NetworkTransformReliable networkTransform;
        [SerializeField] private Collider itemCollider;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private MeshRenderer[] renderers;

        public ItemScriptebleObject item;
        private Vector3 originalScale;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        public void ResetScale()
        {
            transform.localScale = originalScale;
        }

        [ClientRpc]
        public void RpcRestoreWorldState()
        {
            ResetScale();
            gameObject.layer = item.DefaultLayer;
        }

        public void SetVisible(bool value)
        {
            if (!isServer)
                return;
        }

        public void ShowVisual()
        {
            RpcShowVisual();
        }

        public void HideVisual()
        {
            RpcHideVisual();
        }

        [ClientRpc]
        private void RpcShowVisual()
        {
            Debug.Log($"SHOW {name}");
            foreach (var renderer in renderers)
            {
                renderer.enabled = true;
            }

            itemCollider.enabled = false;
        }

        [ClientRpc]
        private void RpcHideVisual() 
        {
            Debug.Log($"HIDE {name}");
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
            //if (value)
            //{
            //    ShowVisual();
            //}
            //else
            //{
            //    HideVisual();
            //}
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
