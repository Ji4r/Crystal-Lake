using System;
using UnityEngine;
using TriInspector;

namespace MyProj
{
    [CreateAssetMenu(fileName = "DifficultyGameData", menuName = "SO/DifficultyGameData")]
    public class DifficultyGameData : ScriptableObject
    {
        [Title("Difficulty Settings")]

        [Header("Spawn prop"), Space(2)]
        [SerializeField, Range(0, 100), Tooltip("Максимальный процент для расчёта")]
        private byte maxPercentageOfSpawn; // Не подключено
        [SerializeField, Range(0, 100), Tooltip("Процент спавна хилок")]
        private byte percentageOfSpawnHigh; // Не подключено
        [SerializeField, Range(0, 100), Tooltip("Процент спавна батареек")]
        private byte percentageOfBatterySpawn; // Не подключено
        [SerializeField, Range(0, 100), Tooltip("Процент спавна вспомогательных предметов")]
        private byte percentageOfSpawnAuxiliaryItems; // Не подключено

        [Header("Microphone Settings"), Space(2)]
        [SerializeField] private bool isEnabledAlways; // Не подключено

        [Header("Impact on systems"), Space(2)]
        [SerializeField] private RangeByte countNotesForKeypad; // Не подключено
        [SerializeField, Tooltip("Количество ошибок в кодовом замке перед сиреной")]
        private byte numberOfErrorsInKeypadBeforeSiren; // Не подключено
        [SerializeField] private byte nicknameDisplayDistance; // Не подключено
        [SerializeField, Tooltip("Шанс на кашель от сигарет")]
        private RangeByte chanceOfCoughingFromCigarettes; // Не подключено 
        [SerializeField, Tooltip("Шанс на отрыжку от пива")]
        private RangeByte chanceToBurpFromBeer; // Не подключено

        [Header("Ai Enemy"), Space(2)]
        [SerializeField] private float speedWalkingEnemy; // Не подключено
        [SerializeField] private RangeByte damageEnemy; // Не подключено
        //Реакция на события - может не сразу среагировать (например топнул пару раз)

        [Header("Influence on the player"), Space(2)]
        [SerializeField, Tooltip("Множитель шума игрока")]
        [Range(1, 3)] private float playersNoiseMultiplier; 
        [SerializeField, Tooltip("Переход между состоянием из идл в ходьбу")] 
        private float transitionBetweenIdlAndWlking;
        [SerializeField, Tooltip("Переход между состоянием из ходьбы в бег")]
        private float transitionBetweenWalkingAndRunning;
        [SerializeField, Tooltip("Расход стамины при беге")]
        private byte staminaConsumptionWhenRunning; 
        [SerializeField, Tooltip("Расход стамины при прыжке")]
        private byte staminaConsumptionWhenJump; 
        [SerializeField, Tooltip("Скорость восстановления в idle")]
        private byte idleRecoveryRate;
        [SerializeField, Tooltip("Скорость восстановления в Move")]
        private byte moveRecoveryRate;
        [SerializeField, Tooltip("Дистанция зрения")]
        private float FieldOfView; // Не подключено


        [Header("UI"), Space(2)]
        [SerializeField] private bool showVoiceVolume;
        [SerializeField] private bool showStaminaLevel;
        [SerializeField] private bool showBatteryPercentage; 


        public byte PercentageOfSpawnHigh => percentageOfSpawnHigh;
        public byte PercentageOfBatterySpawn => percentageOfBatterySpawn;
        public byte PercentageOfSpawnAuxiliaryItems => percentageOfSpawnAuxiliaryItems;
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
        public byte IdleRecoveryRate  => idleRecoveryRate; 
        public byte MoveRecoveryRate  => moveRecoveryRate;
        public float FieldOfView1 => FieldOfView;
        public bool ShowVoiceVolume => showVoiceVolume;
        public bool ShowStaminaLevel => showStaminaLevel;
        public bool ShowBatteryPercentage => showBatteryPercentage;
        public byte MaxPercentageOfSpawn => maxPercentageOfSpawn;
    }
}