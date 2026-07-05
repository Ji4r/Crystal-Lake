using UnityEngine;

namespace MyProj
{
    public class PackOfCigarettes : ItemUse
    {
        [SerializeField, Min(0)] private int addHealth;

        [SerializeField] private float durationOfCoughEffect;
        [SerializeField] private RangeFloat coughResponseTime;

        public override void Use(Camera gameCamera, AllPartPlayer player)
        {
            if (ItemWasUsed)
                return;

            player.Get<HealthManager>().AddHealth(addHealth);
            player.Get<EffectsOnPlayer>().AddEffects<CoughEffect>(() =>
            {
                return new CoughEffect(player.Get<EffectsOnPlayer>(), player.Get<CharacterNoise>(), durationOfCoughEffect, coughResponseTime);
            });

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
