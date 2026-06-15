using System;
using UnityEngine;
using Mirror;
using TriInspector;

namespace MyProj
{
    public class HealthManager : NetworkBehaviour, IPartPlayer
    {
        [Header("Право созидателя")]
        [SerializeField, Range(0,100)] private int currentHealthOnInspector;
        [Header("Health parameters")]
        [SerializeField] private int maxHealth;

        public Action OnDeathServer;
        public Action OnDeathClient;
        public Action ResetParameters;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;

        [SyncVar(hook = nameof(OnCurrentHealthChanged))] private int currentHealth;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            currentHealth = currentHealthOnInspector;
        }
#endif

        public override void OnStartServer()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0) return;

            CmdTakeDamage(damage);
        }

        public void AddHealth(int amount)
        {
            if (amount <= 0) return;

            CmdAddHealth(amount);
        }

        [Command]
        private void CmdTakeDamage(int damage)
        {
            if (damage >= currentHealth)
            {
                currentHealth = 0;
                OnDeathServer?.Invoke();
                return;
            }

            currentHealth -= damage;
        }

        [Command]
        private void CmdAddHealth(int amount)
        {
            if (currentHealth + amount >= maxHealth)
            {
                currentHealth = maxHealth;
                return;
            }

            currentHealth += amount;
        }

        private void OnCurrentHealthChanged(int oldHealth, int newHealth)
        {
            if (!isOwned)
                return;


            if (newHealth <= 0)
            {
                OnDeathClient?.Invoke();
            }
        }
    }
}
