using UnityEngine;
using Mirror;

namespace MyProj
{
    public class KeyForDoor : ItemUse
    {
        [SerializeField] private Camera cameraMain;
        [SerializeField] private LayerMask keyMask;

        private void OnEnable()
        {
            cameraMain = Camera.main;
            Debug.Log(cameraMain.name + " fdfdsf");
        }

        public override void Use(Camera gameCamera, NetworkIdentity player, QuickSlotInventory inventory)
        {
            Debug.Log(cameraMain.transform.localPosition);
            Ray ray = new Ray(cameraMain.transform.position, cameraMain.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 5, Color.red, 1f);

            Physics.Raycast(ray, out var hitInfo, 5, keyMask);
            Debug.Log(hitInfo.collider.name);

            if (hitInfo.collider == null)
                return;

            if (hitInfo.collider.TryGetComponent<DoorLock>(out var doorLock))
            {
                doorLock.OpenDoorLock();
                Debug.Log(doorLock.isOpen + "Дверь");
            }
        }
    }
}
