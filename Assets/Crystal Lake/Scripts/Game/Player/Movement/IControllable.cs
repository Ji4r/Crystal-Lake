using UnityEngine;

namespace MyProj
{
    public interface IControllable
    {
        public void Move(Vector3 direction);
        public void Jump();
        public void Crounch();
        public void Sprint();
        public void CanceledSprint();
        public void Interact();
        public void DropProp();
        public void UseProp();
        public void UseFlashlight();
        public void Exit();
        public void TabAnitimation();
        public void SwitchSlot1();
        public void SwitchSlot2();
        public void SwitchSlot3();
        public void SwitchSlot4();
    }
}
