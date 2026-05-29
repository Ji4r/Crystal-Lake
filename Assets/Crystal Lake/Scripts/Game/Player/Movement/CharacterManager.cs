using Mirror;
using UnityEngine;

namespace MyProj
{
    [RequireComponent(typeof(CharacterController), 
        typeof(AllPartPlayer),
        typeof(IInputReader))]
    public class CharacterManager : NetworkBehaviour, IControllable, ILocalOnly
    {
        [SerializeField] private AllPartPlayer allPartPlayer;

        private CharacterInteracter characterInteracter;
        private CharacterMovement characterMovement;
        private CharacterGravity characterGravity;
        private Inventory inventory;
        private QuickSlotInventory quickSlotInventory;
        private CharacterAnimator characterAnimator;
        private Flashlight flashlight;
        private UseProp useProp;

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
            characterAnimator = allPartPlayer.Get<CharacterAnimator>();
            useProp = allPartPlayer.Get<UseProp>();
            flashlight = allPartPlayer.Get<Flashlight>();
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

            CmdDropItem();
        }

        public void SwitchSlot1()
        {
            if (!canControl) return;
            quickSlotInventory.CmdSetActiveSlot(0);
        }

        public void SwitchSlot2()
        {
            if (!canControl) return;
            quickSlotInventory.CmdSetActiveSlot(1);
        }

        public void SwitchSlot3()
        {
            if (!canControl) return;
            quickSlotInventory.CmdSetActiveSlot(2);
        }

        public void SwitchSlot4()
        {
            if (!canControl) return;
            quickSlotInventory.CmdSetActiveSlot(3);
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

        [Command]
        private void CmdPickupItem(NetworkIdentity itemIdentity)
        {
            if (itemIdentity == null)
                return;

            if (!itemIdentity.TryGetComponent<Item>(out var item))
                return;

            inventory.AddItem(item.item, item.gameObject);
            RpcAttachItem(itemIdentity, netIdentity);
            item.RpcSetVisible(false);
        }

        [Command]
        private void CmdDropItem()
        {
            var gameObj = quickSlotInventory.GetCurrentProp();

            if (gameObj == null)
                return;

            Item item = gameObj.GetComponent<Item>();

            if (item == null)
                return;

            item.RpcSetVisible(true);
            item.RpcDropItem();
            inventory.DropItem();
        }

        [ClientRpc]
        private void RpcAttachItem(NetworkIdentity itemIdentity, NetworkIdentity playerIdentity)
        {
            var itemObj = itemIdentity.gameObject;
            var player = playerIdentity.GetComponent<CharacterManager>();
            player.quickSlotInventory.SetParentFromProp(itemObj);
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