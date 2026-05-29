using System;
using UnityEngine;

namespace MyProj
{
    public class HealthManager : MonoBehaviour, IPartPlayer
    {
        [SerializeField] private byte maxHealth;

        public Action OnDeath;
        public Action ResetParameters;

        public byte MaxHealth => maxHealth;
        public byte CurrentHealth => currentHealth;

        private byte currentHealth;
    }
}
