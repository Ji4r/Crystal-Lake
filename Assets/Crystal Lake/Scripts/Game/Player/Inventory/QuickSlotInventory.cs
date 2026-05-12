using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MyProj
{
    [RequireComponent(typeof(Inventory))]
    public class QuickSlotInventory : MonoBehaviour, IPartPlayer
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


        private void Start()
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

            if (slots.Count != 0)
            {
                SetActiveSlot(0);
            }

            originalSize = new List<Vector3>();

            for (int i = 0; i < slots.Count; i++)
            {
                originalSize.Add(Vector3.zero);
            }
        }

        public void SetActiveSlot(int indexSlot)
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
            SetPropInHandle(indexSlot);
        }

        public int GetActiveSlot()
        {
            return lastActiveSlot;
        }

        public void SetParentFromProp(GameObject prop)
        {
            prop.transform.SetParent(positionSpawnPropInHandle);
        }

        public GameObject SetPropInHandle(int indexSlot)
        {
            if (slots[indexSlot].Item == null)
            { return null; }

            if (slots[indexSlot].Item.Prefab == null)
            { return null; }

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
                return null;

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
            return gameObj;
        }

        public GameObject GetCurrentProp()
        {
            return currentPropInHandle;
        }

        public void DissablePropInHandle()
        {
            if (isHandisOccupied == false)
            {
                currentPropInHandle?.SetActive(false);
                currentPropInHandle = null;
                isHandisOccupied = true;
            }
        }

        public void DestroyPropInHandle()
        {
            if (isHandisOccupied == false)
            {
                slots[lastActiveSlot].SetItem(null);
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