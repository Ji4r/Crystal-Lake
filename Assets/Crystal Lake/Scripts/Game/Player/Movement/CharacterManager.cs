using Mirror;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

namespace MyProj
{
    [RequireComponent(typeof(CharacterController), 
        typeof(AllPartPlayer),
        typeof(IInputReader))]
    public class CharacterManager : NetworkBehaviour, IControllable, ILocalOnly
    {
        [SerializeField] private AllPartPlayer allPartPlayer;
        [SerializeField] private Camera playerCamera;

        private CharacterInteracter characterInteracter;
        private CharacterMovement characterMovement;
        private CharacterGravity characterGravity;
        private InventoryData inventoryData;
        private InventoryState inventoryState;
        private Inventory inventory;
        private QuickSlotInventory quickSlotInventory;
        private HandController handController;
        private Flashlight flashlight;
        private UseProp useProp;
        private CircularAnimationMenu circularAnimationMenu;

        private IInteractible interactibleObject;
        private RaycastHit hitInfo;

        private bool canControl = true;

        private IExitHandler exitHandler;

        private void Awake()
        {
            characterInteracter = allPartPlayer.Get<CharacterInteracter>();
            characterMovement = allPartPlayer.Get<CharacterMovement>();
            characterGravity = allPartPlayer.Get<CharacterGravity>();
            inventory = allPartPlayer.Get<Inventory>();
            quickSlotInventory = allPartPlayer.Get<QuickSlotInventory>();
            useProp = allPartPlayer.Get<UseProp>();
            flashlight = allPartPlayer.Get<Flashlight>();
            inventoryData = allPartPlayer.Get<InventoryData>();
            inventoryState = allPartPlayer.Get<InventoryState>();
            handController = allPartPlayer.Get<HandController>();
            circularAnimationMenu = allPartPlayer.Get<CircularAnimationMenu>();
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }

        private void Update()
        {
            var hit = characterInteracter.CheckInteract();
            interactibleObject = hit.Item1;
            hitInfo = hit.Item2;
        }

        public void Move(Vector3 direction)
        {
            if (!canControl) return;
            characterMovement.Move(direction);
        }

        public void Sprint()
        {
            if (!canControl) return;
            characterMovement.Sprint();
        }

        public void CanceledSprint()
        {
            if (!canControl) return;
            characterMovement.CanceledSprint();
        }

        public void Jump()
        {
            if (!canControl) return;
            characterGravity.Jump();
        }

        public void Interact()
        {
            if (!canControl) return;
            Debug.Log("Interact - " + interactibleObject);
            if (interactibleObject == null)
                return;

            if (interactibleObject is Item item)
            {
                CmdPickupItem(item.netIdentity);
            }
            else
            {
                interactibleObject.Interact(hitInfo, allPartPlayer);
            }
        }

        public void SetControl(bool value)
        {
            canControl = value;
        }

        public void Crounch()
        {
            if (!canControl) return;
            characterMovement.Crounch();
        }

        public void DropProp()
        {
            if (!canControl)
                return;

            Vector3 throwDirection = (playerCamera.transform.forward + Vector3.up * 0.15f).normalized;

            CmdDropItem(inventory.posDropItem.position, throwDirection, inventory.throwForce);
        }

        public void SwitchSlot1()
        {
            if (!canControl) return;
            quickSlotInventory.SetActiveSlot(0);
        }

        public void SwitchSlot2()
        {
            if (!canControl) return;
            quickSlotInventory.SetActiveSlot(1);
        }

        public void SwitchSlot3()
        {
            if (!canControl) return;
            quickSlotInventory.SetActiveSlot(2);
        }

        public void SwitchSlot4()
        {
            if (!canControl) return;
            quickSlotInventory.SetActiveSlot(3);
        }

        public void UseProp()
        {
            Debug.Log("Use");
            useProp.UsePropInHandle();
        }

        public void UseFlashlight()
        {
            flashlight.UseFlashlight();
        }

        public void TabAnitimation()
        {
            circularAnimationMenu.OpenTabCircle();
        }

        [Command]
        private void CmdPickupItem(NetworkIdentity itemIdentity)
        {
            if (itemIdentity == null)
                return;

            if (!itemIdentity.TryGetComponent<Item>(out var item))
                return;

            int slot = inventoryData.TryAddItem(itemIdentity.netId);

            if (slot == -1)
                return;

            item.RpcSetVisible(false);

            if (slot == inventoryState.ActiveSlot)
            {
                inventoryState.ActiveItemNetId = itemIdentity.netId;
                return;
            }

            itemIdentity.transform.SetParent(handController.handPoint);
            item.HideVisual();        
        }


        [Command]
        private void CmdDropItem(Vector3 dropPosition, Vector3 throwDirection, float throwForce)
        {
            int activeSlot = inventoryState.ActiveSlot;

            uint itemNetId = inventoryData.RemoveItem(activeSlot);
            inventoryState.ActiveItemNetId = 0;

            if (itemNetId == 0)
                return;

            if (!NetworkServer.spawned.TryGetValue(itemNetId, out var identity))
                return;

            GameObject gameObj = identity.gameObject;

            Item item = gameObj.GetComponent<Item>();

            if (item == null)
                return;

            gameObj.transform.SetParent(null);

            item.RpcRestoreWorldState();
            gameObj.transform.position = dropPosition;

            Rigidbody rb = gameObj.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.linearVelocity = throwDirection * throwForce;

            item.RpcSetVisible(true);
            item.ShowVisual();
            item.RpcDropItem();
        }

        /// <summary>
        /// Удоление предмета из инвентаря после его использования. Вызывается на сервере.
        /// </summary>
        [Command]
        public void CmdConsumeActiveItem() 
        {
            int slot = inventoryState.ActiveSlot;

            uint itemId = inventoryData.RemoveItem(slot);

            if (itemId == 0)
                return;

            inventoryState.ActiveItemNetId = 0;
        }


        public void Exit()
        {
            exitHandler?.Exit();
        }

        public void SetExitHandler(IExitHandler handler)
        {
            if (handler == null)
            {
                Debug.LogWarning("Попытка установить null обработчику выхода");
                return;
            }

            exitHandler = handler;
            Debug.Log($"SetExitHandler - {exitHandler}");
        }

        public void ClearExitHandler(IExitHandler handler)
        {
            if (exitHandler == handler)
                exitHandler = null;

            //Устонавливаем базовый ui чтобы при нажатии у нас открывалось меню паузы.
        }
    }
}