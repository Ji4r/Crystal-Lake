using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public enum TypeOfActivity{
        Running, Jumping
    }

    [System.Serializable]
    public class EnergyConsumptionData {
        public TypeOfActivity TypeOfActivity;
        public float EnergyConsumption;
    }

    [RequireComponent(
        typeof(CharacterGravity),
        typeof(HealthManager))]
    public class StaminaController : MonoBehaviour, IPartPlayer
    {
        [Header("Настройка восстановления стамины")]
        [SerializeField, Tooltip("Раз во сколько секунд восстанавливается стамина")]
        private float staminaRecoveryTime;
        [SerializeField, Tooltip("Сколько стамины будет восстанавливаться в секунду в idle")]
        private float staminaRecoveryIdleState;
        [SerializeField, Tooltip("Сколько стамины будет восстанавливаться в секунду в движении")]
        private float staminaRecoveryMoveState;

        [Header("Расход энергии")]
        [SerializeField] private List<EnergyConsumptionData> energyConsumptionAtList;

        [Header("Настройки стамины")]
        [SerializeField] private float maxStamina = 100f;
        public bool UseEndlessStamina = false;

        public event Action<float, float> StaminaChanged;

        public float StaminaRecoveryTime => staminaRecoveryTime;
        public float CurrentStamina => currentStamina;
        public float MaxStamina => maxStamina;

        private float currentStamina;
        private Coroutine recoveryStamina;
        private WaitForSeconds recoveryStaminaTime;
        private Dictionary<TypeOfActivity, float> energyConsumptionAndActivityType;
        private CharacterMovement characterMovement;
        private CharacterGravity characterGravity;
        private HealthManager healthManager;

        private void Awake()
        {
            Initialized();

            currentStamina = maxStamina;

            recoveryStaminaTime = new WaitForSeconds(staminaRecoveryTime);
        }

        private void OnEnable()
        {
            healthManager.OnDeath += PlayerOnDeath;

            characterMovement.ChangeStaminaAction += TryToWriteOffStamina;
            characterGravity.ChangeStaminaAction += TryToWriteOffStamina;
        }

        private void OnDisable()
        {
            healthManager.OnDeath -= PlayerOnDeath;

            characterMovement.ChangeStaminaAction -= TryToWriteOffStamina;
            characterGravity.ChangeStaminaAction -= TryToWriteOffStamina;

            if (recoveryStamina != null)
            {
                StopCoroutine(recoveryStamina);
                recoveryStamina = null;
            }
        }

        private void Initialized()
        {
            characterMovement = GetComponent<CharacterMovement>();
            characterGravity = GetComponent<CharacterGravity>();
            healthManager = GetComponent<HealthManager>();

            energyConsumptionAndActivityType = new(energyConsumptionAtList.Count);

            foreach (var item in energyConsumptionAtList)
            {
                energyConsumptionAndActivityType.Add(item.TypeOfActivity, item.EnergyConsumption);
            }
        }

        private IEnumerator RecoveryStamina()
        {
            while (true)
            {
                yield return recoveryStaminaTime;

                if (!characterMovement.IsSprint && CurrentStamina < maxStamina)
                {
                    if (!characterMovement.IsMoving)
                        currentStamina += staminaRecoveryIdleState;
                    else
                        currentStamina += staminaRecoveryMoveState;

                    currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

                    StaminaChanged?.Invoke(maxStamina, currentStamina);
                }
            }
        }

        public void ResetStats()
        {
            currentStamina = maxStamina;
            StaminaChanged?.Invoke(maxStamina, currentStamina);
        }

        private bool TryToWriteOffStamina(TypeOfActivity typeActivity)
        {
            if (UseEndlessStamina)
                return true;

            if (!energyConsumptionAndActivityType.TryGetValue(typeActivity, out float writeOff))
            {
                throw new Exception($"Не найден такой тип активности как - {typeActivity}");
            }

            switch (typeActivity)
            {
                case TypeOfActivity.Running:
                    writeOff *= Time.deltaTime; 
                    break;
            }

            if (CurrentStamina - writeOff <= 0)
                return false;

            if (recoveryStamina != null)
            {
                StopCoroutine(recoveryStamina);
                recoveryStamina = null;
            }

            currentStamina -= writeOff;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            StaminaChanged?.Invoke(maxStamina, currentStamina);

            if (recoveryStamina == null)
            {
                recoveryStamina = StartCoroutine(RecoveryStamina());
            }

            return true;
        }

        private void PlayerOnDeath()
        {
            if (recoveryStamina != null)
            {
                StopCoroutine(recoveryStamina);
                recoveryStamina = null;
            }
        }
    }
}