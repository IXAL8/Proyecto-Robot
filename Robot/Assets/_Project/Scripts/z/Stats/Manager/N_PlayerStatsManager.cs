using System;
using UnityEngine;

namespace Robot
{
    public class N_PlayerStatsManager : Singleton<IStatsSource>, IStatsSource
    {
        [Header("Base Stats")]
        [SerializeField] private float _baseMaxHealth;
        [SerializeField] private float _baseDamage;
        [SerializeField] private float _baseMovementSpeed;

        [Header("Bonus Stats")]
        private float _bonusMaxHealth, _bonusDamage, _bonusMoveSpeed;

        public float Attack => _baseDamage + _bonusDamage;
        public float MaxHealth => _baseMaxHealth + _bonusMaxHealth;
        public float MoveSpeed => _baseMovementSpeed + _bonusMoveSpeed;

        public event Action OnStatsChanged;
        public event Action<float> OnHealthChanged;
        public event Action OnPlayerDied;

        public void NotifyHealthChanged(float currentHealth) => OnHealthChanged?.Invoke(currentHealth);

        public void NotifyPlayerDied() => OnPlayerDied?.Invoke();

        #region Chips Events
        public void AddAttackBonus(float amount) { _bonusDamage += amount; OnStatsChanged?.Invoke(); }
        public void RemoveAttackBonus(float amount) { _bonusDamage -= amount; OnStatsChanged?.Invoke(); }

        public void AddMoveSpeedBonus(float amount) { _bonusMoveSpeed += amount; OnStatsChanged?.Invoke(); }
        public void RemoveMoveSpeedBonus(float amount) { _bonusMoveSpeed -= amount; OnStatsChanged?.Invoke(); }

        public void AddMaxHealthBonus(float amount) { _bonusMaxHealth += amount; OnStatsChanged?.Invoke(); }
        public void RemoveMaxHealthBonus(float amount) { _bonusMaxHealth -= amount; OnStatsChanged?.Invoke(); }

        #endregion
    }
}

