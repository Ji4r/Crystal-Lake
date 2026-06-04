using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public enum TypeDiffecaltyGame
    {
        None,
        Walking,
        Terrible, 
        Nightmare,
        Horror
    }

    public class DifficultyGame : NetworkBehaviour
    {
        public static DifficultyGame Instance;

        [SerializeField] private DifficultyGameData walking;
        [SerializeField] private DifficultyGameData terrible;
        [SerializeField] private DifficultyGameData nightmare;
        [SerializeField] private DifficultyGameData horror;

        [SyncVar(hook = nameof(OnDifficultyChanged))]
        private TypeDiffecaltyGame difficulty;

        private Dictionary<TypeDiffecaltyGame, DifficultyGameData>
            difficultyData;

        public DifficultyGameData Current
        { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);

            difficultyData = new()
            {
                { TypeDiffecaltyGame.Walking, walking },
                { TypeDiffecaltyGame.Terrible, terrible },
                { TypeDiffecaltyGame.Nightmare, nightmare },
                { TypeDiffecaltyGame.Horror, horror }
            };
        }

        public void SetDifficulty(TypeDiffecaltyGame newDifficulty)
        {
            difficulty = newDifficulty;
        }

        void OnDifficultyChanged(TypeDiffecaltyGame oldValue, TypeDiffecaltyGame newValue)
        {
            if (difficultyData.TryGetValue(newValue, out var difficulty))
            {
                Current = difficulty;
                Debug.Log($"New Difficulty = {newValue}");
            }
            else
            {
                Current = walking;
                Debug.LogError($"Братка Difficulty {newValue} not found, defaulting to Walking");
            }
        }
    }
}
