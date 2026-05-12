using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyProj
{
    public class ShakeManager : MonoBehaviour, ILocalOnly
    {
        [SerializeField] private CharacterMovement characterMovement;
        [SerializeField] private Camera playerCamera;

        [SerializeField] private ScriptableShakeCamera presetIdle;
        [SerializeField] private ScriptableShakeCamera presetWalk;
        [SerializeField] private ScriptableShakeCamera presetSprint;
        [SerializeField] private ScriptableShakeCamera presetCrounch;

        private ShakeState currentShake;
        private Dictionary<Type, ShakeState> statesShake;

        private void Init()
        {
            statesShake = new Dictionary<Type, ShakeState>
            {
                {typeof(ShakeIdle), new ShakeIdle(playerCamera, presetIdle)},
                {typeof(ShakeWalk), new ShakeWalk(playerCamera, presetWalk)},
                {typeof(ShakeSprint), new ShakeSprint(playerCamera, presetSprint)},
                {typeof(ShakeCrounch), new ShakeCrounch(playerCamera, presetCrounch)}
            };
        }

        private void Awake()
        {
            Init();
            ChangeStateShake<ShakeIdle>(true);
        }

        private void OnEnable()
        {
            characterMovement.OnStartWalking += HandleStartWalking;
            characterMovement.OnStartSprinting += HandleStartSprinting;
            characterMovement.OnStartCrouching += HandleStartCrouching;
            characterMovement.OnStopMoving += HandleStopMoving;
            characterMovement.OnStopSprinting += HandleStopSprinting;
        }

        private void OnDisable()
        {
            currentShake?.Dispose();
            characterMovement.OnStartWalking -= HandleStartWalking;
            characterMovement.OnStartSprinting -= HandleStartSprinting;
            characterMovement.OnStartCrouching -= HandleStartCrouching;
            characterMovement.OnStopMoving -= HandleStopMoving;
            characterMovement.OnStopSprinting -= HandleStopSprinting;
        }

        private void HandleStartWalking() => ChangeStateShake<ShakeWalk>(true);
        private void HandleStartSprinting() => ChangeStateShake<ShakeSprint>(true);
        private void HandleStartCrouching() => ChangeStateShake<ShakeCrounch>(true);

        private void HandleStopMoving()
        {
            if (!characterMovement.IsSprint)
                ChangeStateShake<ShakeIdle>(true);
        }

        private void HandleStopSprinting()
        {
            if (characterMovement.IsMoving)
                ChangeStateShake<ShakeWalk>(true);
            else
                ChangeStateShake<ShakeIdle>(true);
        }

        public void StartShake()
        {
            currentShake.EnabledShake();
        }

        public void StopShake()
        {
            if (currentShake != null)
                currentShake.DissabledShake();
        }

        public void ChangeStateShake<T>(bool isStartPlay = true) where T : ShakeState
        {
            if (!statesShake.TryGetValue(typeof(T), out var state))
                throw new System.Exception($"Not found state sheke camera {typeof(T)}");

            if (currentShake != null)
                if (state.GetType() == currentShake.GetType())
                    return;

            StopShake();
            currentShake = state;
            if (isStartPlay && !currentShake.isPlay)
                StartShake();
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }
    }
}