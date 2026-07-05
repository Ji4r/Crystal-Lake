using UnityEngine;

namespace MyProj
{
    public class FirstAidKit : ItemUse
    {
        [SerializeField, Min(0)] private int addHealth;

        public override void Use(Camera gameCamera, AllPartPlayer player)
        {
            if (ItemWasUsed)
                return;

            player.Get<HealthManager>().AddHealth(addHealth);

            ItemWasUsed = true;


            if (IsDisposable)
            {
                ItemWasUsed = true;

                var characterManager = player.Get<CharacterManager>();
                characterManager.CmdConsumeActiveItem();
            }
        }
    }
}
