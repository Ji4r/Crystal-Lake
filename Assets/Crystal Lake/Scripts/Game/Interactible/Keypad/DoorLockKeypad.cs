using UnityEngine;

namespace MyProj
{
    public class DoorLockKeypad : DoorLock, IUseKeypadCode
    {
        public override void OpenDoorLock()
        {
            isOpen = !isOpen;
            Debug.Log(isOpen);
        }

        public void OpenLock()
        {
            OpenDoorLock();
        }
    }
}
