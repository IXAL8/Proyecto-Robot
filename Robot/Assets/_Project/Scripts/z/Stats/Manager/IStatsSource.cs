using System;
using UnityEngine;

namespace Robot
{
    public interface IStatsSource
    {
        float Attack {  get;}
        float MaxHealth { get;}
        float MoveSpeed { get;}

        event Action OnStatsChanged;
        event Action<float> OnHealthChanged;
        event Action OnPlayerDied;

        void NotifyHealthChanged(float currentHealth);
        void NotifyPlayerDied();

        #region Chips Events
        void AddAttackBonus(float amount);
        void RemoveAttackBonus(float amount);

        void AddMaxHealthBonus(float amount);
        void RemoveMaxHealthBonus(float amount);

        void AddMoveSpeedBonus(float amount);
        void RemoveMoveSpeedBonus(float amount);
        #endregion
    }
}
