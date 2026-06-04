using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace MyProj
{
    public class Flashlight : NetworkBehaviour, IPartPlayer
    {
        [SerializeField] private AllPartPlayer allPartPlayer;
        [SerializeField] Transform cameraTransform;
        [SerializeField] private LayerMask ignoreTypeWalls;
        [SerializeField] private float maxDistanceToWall = 0.4f;

        [Header("Flashlight")]
        [SerializeField] private float maxBatteryLevel = 100f;
        [SerializeField] private float batteryConsumptionOneStep = 0.25f;
        [SerializeField] private float timeOneStepBatteryConsumption = 0.5f;
        [SerializeField] private Light localLight;
        [SerializeField] private Light globalLight;
        [SerializeField] private VolumetricAdditionalLight volumetricAdditionalLight;
        [SerializeField] private GameObject conusLight;

        [SyncVar(hook = nameof(ChangeEnableFlashlight))]
        private bool flashlightIsEnabled;

        public Action<float, float> ChangeBatteryLevel;

        private float batteryLevel;
        private CharacterHandAnimator animator;
        private bool handIsUp;
        private Coroutine coroutineActiveFlashlight;
        private WaitForSeconds timerCoroutineFlashlight;

        void Start()
        {
            localLight.enabled = false;
            globalLight.enabled = false;
            volumetricAdditionalLight.enabled = false;
            conusLight.SetActive(false);

            batteryLevel = maxBatteryLevel;
            ChangeBatteryLevel?.Invoke(maxBatteryLevel, batteryLevel);
            handIsUp = false;
            animator = allPartPlayer.Get<CharacterHandAnimator>();
            timerCoroutineFlashlight = new WaitForSeconds(timeOneStepBatteryConsumption);
        }

        public void AddBatteryLevel(float amount)
        {
            CmdAddBatteryLevel(amount);
        }

        [Command]
        private void CmdAddBatteryLevel(float amount)
        {
            batteryLevel += amount;
            if (batteryLevel > maxBatteryLevel)
                batteryLevel = maxBatteryLevel;
            ChangeBatteryLevel?.Invoke(maxBatteryLevel, batteryLevel);
        }

        void LateUpdate()
        {
            if (!isLocalPlayer)
                return;

            bool obstacle = Physics.Raycast(cameraTransform.position, cameraTransform.forward, maxDistanceToWall, ~ignoreTypeWalls);

            if (obstacle && !handIsUp)
            {
                animator.SetUpHand();
                handIsUp = true;
            }
            else if (!obstacle && handIsUp)
            {
                animator.SetReturnHand();
                handIsUp = false;
            }
        }

        public void UseFlashlight()
        {
            if (!isLocalPlayer)
                return;

            if (batteryLevel <= 0)
                return;

            CmdToggleFlashlight(!flashlightIsEnabled);
        }

        [Command]
        private void CmdToggleFlashlight(bool value)
        {
            flashlightIsEnabled = value;
        }

        private void ChangeEnableFlashlight(bool oldVal, bool newVal)
        {
            globalLight.enabled = newVal;
            volumetricAdditionalLight.enabled = newVal;
            conusLight.SetActive(newVal);

            if (!isLocalPlayer)
                return;

             localLight.enabled = newVal;

            if (newVal)
            {
                if (coroutineActiveFlashlight == null)
                    coroutineActiveFlashlight = StartCoroutine(ActiveFlashlight());
            }
            else
                if (coroutineActiveFlashlight != null)
                {
                    StopCoroutine(coroutineActiveFlashlight);
                    coroutineActiveFlashlight = null;
                }
        }


        private IEnumerator ActiveFlashlight()
        {
            while (batteryLevel > 0) 
            {
                batteryLevel -= batteryConsumptionOneStep;
                ChangeBatteryLevel?.Invoke(maxBatteryLevel, batteryLevel);
                yield return timerCoroutineFlashlight;
            }

            if (batteryLevel <= 0 && isLocalPlayer)
            {
                ChangeBatteryLevel?.Invoke(maxBatteryLevel, batteryLevel);
                CmdToggleFlashlight(false);
            }

            coroutineActiveFlashlight = null;
        } 
    }
}
