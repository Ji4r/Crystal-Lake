using UnityEngine;
using Mirror;

namespace MyProj
{
    public class Battery : ItemUse
    {
        [Header("Настройки батареи")]
        [SerializeField] private int batteryLevelToAdd = 20;

        public override void Use(Camera gameCamera, AllPartPlayer player)
        {
            if (ItemWasUsed)
                return;

            var flashlight = player.Get<Flashlight>();
            flashlight.AddBatteryLevel(batteryLevelToAdd);


            if (IsDisposable)
            {
                ItemWasUsed = true;

                var characterManager = player.Get<CharacterManager>();
                characterManager.CmdConsumeActiveItem();
            }
        }
    }
}
