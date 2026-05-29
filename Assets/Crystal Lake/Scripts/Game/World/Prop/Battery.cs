using UnityEngine;
using Mirror;

namespace MyProj
{
    public class Battery : ItemUse
    {
        [Header("Настройки батареи")]
        [SerializeField] private int batteryLevelToAdd = 20;

        public override void Use(Camera gameCamera, NetworkIdentity player, QuickSlotInventory inventory)
        {
            if (ItemWasUsed)
                return;

            var allPartPlayer = player.GetComponent<AllPartPlayer>();
            var flashlight = allPartPlayer.Get<Flashlight>();
            flashlight.AddBatteryLevel(batteryLevelToAdd);


            if (IsDisposable)
            {
                ItemWasUsed = true;
                inventory.DestroyPropInHandle();
            }
        }
    }
}
