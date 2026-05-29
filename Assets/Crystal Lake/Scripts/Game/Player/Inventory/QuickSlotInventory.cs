using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MyProj
{
    [RequireComponent(typeof(Inventory))]
    public class QuickSlotInventory : NetworkBehaviour, IPartPlayer
    {
        [SerializeField] private Transform QuickInventoryPanel;
        [SerializeField] private Transform positionSpawnPropInHandle;
        [SerializeField] private int indexPropChildrenCamera;

        [HideInInspector] public List<InventorySlot> slots = new List<InventorySlot>();

        public UnityEvent<int, GameObject> ChangeActiveSlot;
        public List<Vector3> originalSize;

        private GameObject currentPropInHandle;
        private bool isHandisOccupied = true;
        private int lastActiveSlot;

        public override void OnStartLocalPlayer()
        {
            lastActiveSlot = -1;
            isHandisOccupied = true;
            for (int i = 0; i < QuickInventoryPanel.childCount; i++)
            {
                if (QuickInventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
                {
                    slots.Add(QuickInventoryPanel.GetChild(i).GetComponent<InventorySlot>());
                }
            }

            Debug.Log($"Кол-во слотов {slots.Count}");
            if (slots.Count != 0)
            {
                Debug.Log("Слот 0 устоноален");
                CmdSetActiveSlot(0);
            }

            originalSize = new List<Vector3>();

            for (int i = 0; i < slots.Count; i++)
            {
                originalSize.Add(Vector3.zero);
            }
        }

        [Command]
        public void CmdSetActiveSlot(int indexSlot)
        {
            DissablePropInHandle();

            for (int i = 0; i < slots.Count; i++)
            {
                if (i == indexSlot)
                    slots[i].IconSlot.color = Color.gray;
                else
                    slots[i].IconSlot.color = Color.white;
            }

            lastActiveSlot = indexSlot;
            CmdSetPropInHandle(indexSlot);
        }

        public int GetActiveSlot()
        {
            return lastActiveSlot;
        }

        public void SetParentFromProp(GameObject prop)
        {
            prop.transform.SetParent(positionSpawnPropInHandle);
        }

        [Command]
        public void CmdSetPropInHandle(int indexSlot)
        {
            RpcSetPropInHandle(indexSlot);
        }

        [ClientRpc]
        private void RpcSetPropInHandle(int indexSlot)
        {
            if (slots.Count <= indexSlot)
                return;

            if (slots[indexSlot].Item == null)
            { return; }

            if (slots[indexSlot].Item.Prefab == null)
            { return; }

            GameObject gameObj = null;


            foreach (Transform child in positionSpawnPropInHandle)
            {
                if (child.TryGetComponent<IndifecatorSlot>(out var indicator) && indicator.idSlot == indexSlot)
                {
                    gameObj = child.gameObject;
                    break;
                }
            }

            if (gameObj == null)
                return;

            currentPropInHandle = gameObj;

            if (originalSize[indexSlot] == Vector3.zero)
            {
                originalSize[indexSlot] = gameObj.transform.localScale;
            }

            gameObj.transform.localPosition = slots[indexSlot].Item.Position;
            gameObj.transform.localRotation = Quaternion.Euler(slots[indexSlot].Item.Rotation);
            gameObj.transform.localScale = slots[indexSlot].Item.Scale;
            gameObj.layer = slots[indexSlot].Item.SetLayerOnHandle;
            gameObj.GetComponent<Rigidbody>().isKinematic = true;
            gameObj.SetActive(true);
            isHandisOccupied = false;

            var item = gameObj.GetComponent<Item>();
            item.ShowVisual();

            ChangeActiveSlot?.Invoke(lastActiveSlot, gameObj);
        }

        public GameObject GetCurrentProp()
        {
            return currentPropInHandle;
        }

        [Command]
        public void DissablePropInHandle()
        {
            if (isHandisOccupied == false)
            {
                currentPropInHandle?.SetActive(false);
                currentPropInHandle = null;
                isHandisOccupied = true;
            }
        }

        [Command]
        public void DestroyPropInHandle()
        {
            if (isHandisOccupied == false)
            {
                slots[lastActiveSlot].SetItem(null);
                originalSize[lastActiveSlot] = Vector3.zero;
                Destroy(currentPropInHandle);
                currentPropInHandle = null;
                isHandisOccupied = true;
            }
        }

        public void DropProp(int activeSlot)
        {
            if (currentPropInHandle == null)
                return;

            currentPropInHandle.transform.localScale = originalSize[activeSlot];
            originalSize[activeSlot] = Vector3.zero;
            currentPropInHandle.transform.SetParent(null);
            Destroy(currentPropInHandle.GetComponent<IndifecatorSlot>());
            currentPropInHandle = null;
        }
    }
}