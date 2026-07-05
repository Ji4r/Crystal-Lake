using System;
using UnityEngine;

namespace MyProj
{
    public class Beer : ItemUse
    {
        [SerializeField, Min(0)] private int addHealth;
        [SerializeField] private float durationEffectBurp;

        public override void Use(Camera gameCamera, AllPartPlayer player)
        {
            if (ItemWasUsed)
                return;

            player.Get<HealthManager>().AddHealth(addHealth);
            player.Get<EffectsOnPlayer>().AddEffects<BurpEffect>(() => 
            {
                return new BurpEffect(player.Get<EffectsOnPlayer>(), player.Get<CharacterNoise>(), durationEffectBurp); 
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
