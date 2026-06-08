using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyProj
{
    public class ControllableKeyboard : MonoBehaviour, IInputReader, ILocalOnly
    {
        private Vector2 currentMove;
        private Vector2 smoothVelocity;
        [SerializeField] private float smoothTime = 0.15f;

        private GameInput inputSystem;
        private IControllable controllable;
        private IControllableMouse controllableMouse;
        public void Awake()
        {
            if (!gameObject.TryGetComponent(out controllable))
            {
                throw new System.Exception("IControllable Not Found");
            }
            if (!gameObject.TryGetComponent(out controllableMouse))
            {
                throw new System.Exception("IControllableMouse Not Found");
            }

            inputSystem = new GameInput();
            inputSystem.Enable();
        }

        private void OnEnable()
        {
            inputSystem.Gameplay.Jump.performed += JumpPerformed;
            inputSystem.Gameplay.Sprint.performed += SprintPerformed;
            inputSystem.Gameplay.Crouch.performed += CrouchPerformed;
            inputSystem.Gameplay.Sprint.canceled += SprintCanceled;

            inputSystem.Gameplay.Interactive.performed += InteractiblePerformed;
            inputSystem.Gameplay.DropItem.performed += DropItemPerformed;
            inputSystem.Gameplay.UseProp.performed += UsePropPerformed;
            inputSystem.Ui.Exit.performed += ExitPerformed;
            inputSystem.Gameplay.Exit.performed += ExitPerformed;
            inputSystem.Gameplay._1Slot.performed += _1SlotPerformed;
            inputSystem.Gameplay._2Slot.performed += _2SlotPerformed;
            inputSystem.Gameplay._3Slot.performed += _3SlotPerformed;
            inputSystem.Gameplay._4Slot.performed += _4SlotPerformed;
            inputSystem.Gameplay.Flashlight.performed += UseFlashlightPerformed;
            inputSystem.Gameplay.MenuAnims.performed += MenuAnimsPerformed;
        }

        private void OnDisable()
        {
            inputSystem.Disable();
            inputSystem.Gameplay.Jump.performed -= JumpPerformed;
            inputSystem.Gameplay.Sprint.performed -= SprintPerformed;
            inputSystem.Gameplay.Crouch.performed -= CrouchPerformed;
            inputSystem.Gameplay.Sprint.canceled -= SprintCanceled;

            inputSystem.Gameplay.Interactive.performed -= InteractiblePerformed;
            inputSystem.Gameplay.DropItem.performed -= DropItemPerformed;
            inputSystem.Gameplay.UseProp.performed -= UsePropPerformed;
            inputSystem.Gameplay.Exit.performed -= ExitPerformed;
            inputSystem.Gameplay.Flashlight.performed -= UseFlashlightPerformed;
            inputSystem.Ui.Exit.performed -= ExitPerformed;
            inputSystem.Gameplay._1Slot.performed -= _1SlotPerformed;
            inputSystem.Gameplay._2Slot.performed -= _2SlotPerformed;
            inputSystem.Gameplay._3Slot.performed -= _3SlotPerformed;
            inputSystem.Gameplay._4Slot.performed -= _4SlotPerformed;
            inputSystem.Gameplay.MenuAnims.performed -= MenuAnimsPerformed;
        }

        private void OnDestroy()
        {
            if (inputSystem != null)
            {
                inputSystem.Disable();   
                inputSystem.Dispose();
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            ReadMovement();
            ReadMouseLook();
        }

        public void SetActiveMap(string mapName)
        {
            inputSystem.Disable();

            var map = inputSystem.asset.FindActionMap(mapName);
            if (map != null)
            {
                map.Enable();
            }
        }

        public void JumpPerformed(InputAction.CallbackContext obj)
        {
            controllable.Jump();
        }

        public void CrouchPerformed(InputAction.CallbackContext obj)
        {
            controllable.Crounch();
        }

        public void SprintPerformed(InputAction.CallbackContext obj)
        {
           controllable.Sprint();
        }

        public void SprintCanceled(InputAction.CallbackContext obj)
        {
            controllable.CanceledSprint();
        }

        public void InteractiblePerformed(InputAction.CallbackContext obj)
        {
            controllable.Interact();
        }

        public void ReadMovement()
        {
            Vector2 targetMove = inputSystem.Gameplay.Movement.ReadValue<Vector2>();

            currentMove = Vector2.SmoothDamp(currentMove, targetMove, ref smoothVelocity, smoothTime);
            controllable.Move(new Vector3(currentMove.x, 0, currentMove.y));

            //var inputDir = inputSystem.Gameplay.Movement.ReadValue<Vector2>();
            //var direction = new Vector3(inputDir.x, 0, inputDir.y);
            //controllable.Move(direction);
        }

        public void ReadMouseLook()
        {
            var mouseDelta = inputSystem.Gameplay.Look.ReadValue<Vector2>();
            controllableMouse.Look(mouseDelta);
        }

        public void UsePropPerformed(InputAction.CallbackContext obj)
        {
            controllable.UseProp();
        }

        private void UseFlashlightPerformed(InputAction.CallbackContext context)
        {
            controllable.UseFlashlight();
        }

        #region props
        public void DropItemPerformed(InputAction.CallbackContext obj)
        {
            controllable.DropProp();
        }

        public void _1SlotPerformed(InputAction.CallbackContext obj)
        {
            controllable.SwitchSlot1();
        }

        public void _2SlotPerformed(InputAction.CallbackContext obj)
        {
            controllable.SwitchSlot2();
        }

        public void _3SlotPerformed(InputAction.CallbackContext obj)
        {
            controllable.SwitchSlot3();
        }

        public void _4SlotPerformed(InputAction.CallbackContext obj)
        {
            controllable.SwitchSlot4();
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }


        #endregion

        public void ExitPerformed(InputAction.CallbackContext obj)
        {
            controllable.Exit();
        }

        private void MenuAnimsPerformed(InputAction.CallbackContext context)
        {
            controllable.TabAnitimation();
        }
    }
}
