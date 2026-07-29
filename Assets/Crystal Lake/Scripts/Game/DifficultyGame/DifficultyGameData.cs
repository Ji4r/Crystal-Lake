using System;
using TriInspector;
using UnityEngine;

namespace MyProj
{
    [DeclareBoxGroup("SpawnProp", Title = "Spawn prop")]
    [DeclareBoxGroup("MicrophoneSettings", Title = "Microphone Settings")]
    [DeclareBoxGroup("ImpactSystems", Title = "Impact on systems")]
    [DeclareBoxGroup("AiEnemy", Title = "Ai Enemy")]
    [DeclareBoxGroup("InfluencePlayer", Title = "InfluencePlayer")]
    [DeclareBoxGroup("Ui", Title = "UI")]
    [CreateAssetMenu(fileName = "DifficultyGameData", menuName = "SO/DifficultyGameData")]
    public class DifficultyGameData : ScriptableObject
    {
        [Title("Spawn on world")]
        [Group("SpawnProp"), OnValueChanged(nameof(UpdateCurrentPercentage)), SerializeField, Range(0, 100), Tooltip("Максимальный процент для расчёта")]
        private byte maxPercentageOfSpawn; // Не подключено

        [Group("SpawnProp"), OnValueChanged(nameof(UpdateCurrentPercentage)), SerializeField, Range(0, 100), Tooltip("Процент спавна хилок")]
        private byte percentageOfSpawnHigh; // Не подключено
        [Group("SpawnProp"), OnValueChanged(nameof(UpdateCurrentPercentage)), SerializeField, Range(0, 100), Tooltip("Процент спавна батареек")]
        private byte percentageOfBatterySpawn; // Не подключено
        [Group("SpawnProp"), OnValueChanged(nameof(UpdateCurrentPercentage)), SerializeField, Range(0, 100), Tooltip("Процент спавна вспомогательных предметов")]
        private byte percentageOfSpawnAuxiliaryItems; // Не подключено

        [Group("SpawnProp"), ReadOnly, SerializeField] 
        private int currentPercentage;

        [Title("Chest")]
        [Group("SpawnProp"), SerializeField, Range(0, 100), Tooltip("Процент сундуков для заполнения")]
        private byte percentageChestsToFill;

        [Group("SpawnProp"), SerializeField, Tooltip("Процент заполненности сундуков")]
        private RangeByte chestFillPercentage;


        [Group("MicrophoneSettings"), SerializeField] private bool isEnabledAlways; // Не подключено


        [Group("ImpactSystems"), SerializeField] 
        private RangeByte countNotesForKeypad; // Не подключено
        [Group("ImpactSystems"), SerializeField, Tooltip("Количество ошибок в кодовом замке перед сиреной")]
        private byte numberOfErrorsInKeypadBeforeSiren; // Не подключено
        [Group("ImpactSystems"), SerializeField] 
        private byte nicknameDisplayDistance; // Не подключено
        [Group("ImpactSystems"), SerializeField]
        private RangeByte chanceOfCoughingFromCigarettes; // Не подключено 
        [Group("ImpactSystems"), SerializeField]
        private RangeByte chanceToBurpFromBeer; // Не подключено


        [Group("AiEnemy"), SerializeField] private float speedWalkingEnemy; // Не подключено
        [Group("AiEnemy"), SerializeField] private RangeByte damageEnemy; // Не подключено
        //Реакция на события - может не сразу среагировать (например топнул пару раз)


        [Group("InfluencePlayer"), SerializeField, Tooltip("Множитель шума игрока")]
        [Range(1, 3)] private float playersNoiseMultiplier;
        [Group("InfluencePlayer"), SerializeField]
        private float transitionBetweenIdlAndWlking;
        [Group("InfluencePlayer"), SerializeField]
        private float transitionBetweenWalkingAndRunning;
        [Group("InfluencePlayer"), SerializeField]
        private byte staminaConsumptionWhenRunning;
        [Group("InfluencePlayer"), SerializeField]
        private byte staminaConsumptionWhenJump;
        [Group("InfluencePlayer"), SerializeField]
        private byte idleRecoveryRate;
        [Group("InfluencePlayer"), SerializeField]
        private byte moveRecoveryRate;
        [Group("InfluencePlayer"), SerializeField]
        private float FieldOfView; // Не подключено


        [Header("UI"), Space(2)]
        [Group("Ui"), SerializeField] private bool showVoiceVolume;
        [Group("Ui"), SerializeField] private bool showStaminaLevel;
        [Group("Ui"), SerializeField] private bool showBatteryPercentage;


        public byte PercentageOfSpawnHigh => percentageOfSpawnHigh;
        public byte PercentageOfBatterySpawn => percentageOfBatterySpawn;
        public byte PercentageOfSpawnAuxiliaryItems => percentageOfSpawnAuxiliaryItems;
        public byte PercentageChestsToFill => percentageChestsToFill;
        public RangeByte ChestFillPercentage => chestFillPercentage;
        public bool IsEnabledAlways => isEnabledAlways;
        public RangeByte CountNotesForKeypad => countNotesForKeypad;
        public byte NumberOfErrorsInKeypadBeforeSiren => numberOfErrorsInKeypadBeforeSiren;
        public byte NicknameDisplayDistance => nicknameDisplayDistance;
        public RangeByte ChanceOfCoughingFromCigarettes => chanceOfCoughingFromCigarettes;
        public RangeByte ChanceToBurpFromBeer => chanceToBurpFromBeer;
        public float PlayersNoiseMultiplier => playersNoiseMultiplier;
        public float SpeedWalkingEnemy => speedWalkingEnemy;
        public RangeByte DamageEnemy => damageEnemy;
        public float TransitionBetweenIdlAndWlking => transitionBetweenIdlAndWlking;
        public float TransitionBetweenWalkingAndRunning => transitionBetweenWalkingAndRunning;
        public byte StaminaConsumptionWhenRunning => staminaConsumptionWhenRunning;
        public byte StaminaConsumptionWhenJump => staminaConsumptionWhenJump;
        public byte IdleRecoveryRate => idleRecoveryRate;
        public byte MoveRecoveryRate => moveRecoveryRate;
        public float FieldOfView1 => FieldOfView;
        public bool ShowVoiceVolume => showVoiceVolume;
        public bool ShowStaminaLevel => showStaminaLevel;
        public bool ShowBatteryPercentage => showBatteryPercentage;
        public byte MaxPercentageOfSpawn => maxPercentageOfSpawn;



        private void UpdateCurrentPercentage()
        {
            currentPercentage = percentageOfSpawnAuxiliaryItems + percentageOfBatterySpawn + percentageOfSpawnHigh;
            if (currentPercentage > maxPercentageOfSpawn)
            {
                Debug.LogError($"Сумма процентов больше максимального. " +
                    $"Максимальный процент - {maxPercentageOfSpawn}, Сумма - {currentPercentage}", this);
            }
        }
    }
}