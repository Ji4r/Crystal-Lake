using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

namespace MyProj
{
    public class TheEffectOfEndlessStamina : Effect
    {
        StaminaController staminaController;
        private float timeOfAction;

        public TheEffectOfEndlessStamina(StaminaController staminaController, float timeOfAction)
        {
            this.staminaController = staminaController;
            this.timeOfAction = timeOfAction;
        }

        public override async Task EnableEffect()
        {
            Debug.Log("Начало эффекта");
            staminaController.UseEndlessStamina = true;
            await Timer(timeOfAction);
            await DissableEffect();
            Debug.Log("Конец эффекта");
        }

        public override async Task AddEffectTime()
        {
            await base.AddEffectTime();
        }

        public override async Task DissableEffect()
        {
            staminaController.UseEndlessStamina = false;
        }

        private async Task Timer(float time)
        {
            await UniTask.Delay((int)(time * 1000));
        }
    }
}