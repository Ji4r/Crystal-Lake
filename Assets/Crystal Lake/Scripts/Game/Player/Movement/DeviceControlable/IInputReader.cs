using UnityEngine.InputSystem;

namespace MyProj
{
    public interface IInputReader 
    {
        public void SetActiveMap(string mapName);
        public void JumpPerformed(InputAction.CallbackContext obj);
        public void CrouchPerformed(InputAction.CallbackContext obj);
        public void SprintPerformed(InputAction.CallbackContext obj);
        public void SprintCanceled(InputAction.CallbackContext obj);
        public void InteractiblePerformed(InputAction.CallbackContext obj);
        public void ReadMovement();
        public void ReadMouseLook();
        public void UsePropPerformed(InputAction.CallbackContext obj);
        public void DropItemPerformed(InputAction.CallbackContext obj);
        public void _1SlotPerformed(InputAction.CallbackContext obj);
        public void _2SlotPerformed(InputAction.CallbackContext obj);
        public void _3SlotPerformed(InputAction.CallbackContext obj);
        public void _4SlotPerformed(InputAction.CallbackContext obj);
    }
}
