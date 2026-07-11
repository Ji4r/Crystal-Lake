using Mirror;
using UnityEngine;

namespace MyProj
{
    public class HandController : NetworkBehaviour, IPartPlayer
    {
        [SerializeField] private AllPartPlayer allPartPlayer;
        public Transform handPoint;

        public GameObject CurrentItem => currentItem;

        private InventoryState inventoryState;
        private GameObject currentItem;

        private void Awake()
        {
            inventoryState = allPartPlayer.Get<InventoryState>();
        }

        public override void OnStartClient()
        {
            inventoryState.OnActiveItemChangedExternal += ShowItem;

            if (inventoryState.ActiveItemNetId != 0)
            {
                ShowItem(0, inventoryState.ActiveItemNetId);
            }
        }

        public override void OnStopClient()
        {
            inventoryState.OnActiveItemChangedExternal -= ShowItem;
        }

        private void ShowItem(uint oldId, uint newId)
        {
            //Debug.Log($"ShowItem owner={isOwned} local={isLocalPlayer} player={name}");
            //Debug.Log($"[{name}] ShowItem {oldId} -> {newId}");

            if (currentItem != null)
            {
                Item oldItem = currentItem.GetComponent<Item>();

                if (oldItem != null)
                {
                    oldItem.HideVisual();
                }

                currentItem.transform.SetParent(null);

                currentItem = null;
            }

            if (newId == 0)
                return;

            if (!NetworkClient.spawned.TryGetValue(newId, out var identity))
            {
                Debug.Log($"Item {newId} not found");
                return;
            }

            //Debug.Log($"Item {newId} found");

            currentItem = identity.gameObject;
            //Debug.Log($"HandPoint = {(handPoint != null ? handPoint.name : "NULL")}");
            currentItem.transform.SetParent(handPoint, false);

            //Debug.Log($"Parent = {currentItem.transform.parent?.name}");

            Item item = currentItem.GetComponent<Item>();

            if (item == null)
                return;

            currentItem.transform.localPosition = item.item.Position;
            currentItem.transform.localRotation = Quaternion.Euler(item.item.Rotation);
            currentItem.transform.localScale = item.item.Scale;
            //currentItem.layer = item.item.SetLayerOnHandle;
            //Debug.Log($"Position = {currentItem.transform.position}");
            //Debug.Log($"LocalPos = {currentItem.transform.localPosition}");
            //Debug.Log($"WorldPos = {currentItem.transform.position}");

            Rigidbody rb = currentItem.GetComponent<Rigidbody>();

            if (rb != null)
                rb.isKinematic = true;

            item.ShowVisual();
        }
    }
}
