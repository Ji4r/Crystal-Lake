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


        [Command]
        private void CmdPickupItem(NetworkIdentity itemIdentity)
        {
            if (itemIdentity == null)
                return;

            Item item = itemIdentity.GetComponent<Item>();

            inventory.AddItem(item.item, item.gameObject);
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

            inventory.DropItem();
        }

        public void Exit()
        {
            Debug.Log($"Exit - {exitHandler}");
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