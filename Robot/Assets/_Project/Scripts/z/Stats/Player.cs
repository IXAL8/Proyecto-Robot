using UnityEngine;

namespace Robot
{
    public class Player : MonoBehaviour
    {
        [Header("Player Stats")]
        [SerializeField] private float _currentHealth;

        private bool _isDead = false;

        public float CurrentPlayerHealth => _currentHealth;

        private void Start()
        {
            _currentHealth = N_PlayerStatsManager.Source.MaxHealth;
            N_PlayerStatsManager.Source.OnStatsChanged += HandleMaxHealthChange;
            CheckpointSystem.Instance.OnPlayerSpawn += Respawn;
        }

        private void OnDisable()
        {
            N_PlayerStatsManager.Source.OnStatsChanged -= HandleMaxHealthChange;
            CheckpointSystem.Instance.OnPlayerSpawn -= Respawn;
        }

        private void HandleMaxHealthChange()
        {
            _currentHealth = Mathf.Min(_currentHealth, N_PlayerStatsManager.Source.MaxHealth);
            N_PlayerStatsManager.Source.NotifyHealthChanged(_currentHealth);
        }

        public void TakeDamage(float damage)
        {
            if(_isDead) return;

            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            //activar efecto de daño
            N_PlayerStatsManager.Source.NotifyHealthChanged(_currentHealth);

            if (_currentHealth <= 0) Die();
        }

        private void Die()
        {
            _isDead = true;

            N_PlayerStatsManager.Source.NotifyPlayerDied();
            //agregar animacion de muerte
            //agregar spawn
        }

        private void Respawn()
        {
            _isDead = false;

            _currentHealth = N_PlayerStatsManager.Source.MaxHealth;
            //set idle animation
            N_PlayerStatsManager.Source.NotifyHealthChanged(_currentHealth);
        }
    }
}
