using System;
using System.Collections;
using UnityEngine;

namespace Robot
{
    public class PlayerStatsManager : Singleton<IPlayerStats>, IPlayerStats, IAttackable
    {
        [SerializeField] private Animator _playerAnim;
        [SerializeField] private PlayerStats _currentBasePlayerStats;
        [SerializeField] private PlayerStats[] _basePlayerStats;
        [SerializeField] private Renderer playerRenderer;
        [SerializeField] private ItemData _potion;
        [Header("CurrentStats")]
        [SerializeField] private float _currentHealth;
        [SerializeField] private float _currentMaxHealth;
        [SerializeField] private float _currentDamage;
        [SerializeField] private float _currentMovementSpeed;
        [Header("CurrentBaseStats")]
        [SerializeField] private float _baseHealth;
        [SerializeField] private float _baseDamage;
        [SerializeField] private float _baseMovementSpeed;
        [Header("CurrentBonusStats")]
        [SerializeField] private float _bonusHealth;
        [SerializeField] private float _bonusDamage;
        [SerializeField] private float _bonusMovementSpeed;
        [Header("GUIElements")]
        [SerializeField] private GameObject _healthBar, _consumiblesUI;
        [Header("PlayerState")]
        [SerializeField] private bool _isDead;
        [Header("Daño Visual")]

        public float CurrentHealth => _currentHealth;
        public float PlayerMaxHealth => _currentMaxHealth;
        public float PlayerDamage => _currentDamage;
        public float PlayerMovementSpeed => _currentMovementSpeed;

        public event Action<float, float, float> OnBaseStatsChanged;
        public event Action OnDamageRecieved;
        public event Action OnHealRecieved;
        public event Action OnHealthChanges;
        public event Action OnPlayerDeath;
        public event Action<bool> OnMeleeChipActivation;
        public event Action<bool> OnRangeChipActivation;
        public event Action<bool> OnDashChipActivation;
        public event Action<bool> OnHealthBarActivation;
        public event Action<bool> OnConsumiblesUIActivation;

        private void OnEnable()
        {
            InitializeStatManager();
        }

        private void Start()
        {
            ChipInventoryManager.Source.OnChipPlaced += ActiveChipEffects;
            ChipInventoryManager.Source.OnChipRemoved += DeactiveChipEffects;
            InputManager.Source.Consumable1 += HealPlayer;
            RefreshBaseStats();

        }

        private void OnDisable()
        {
            ChipInventoryManager.Source.OnChipPlaced -= ActiveChipEffects;
            ChipInventoryManager.Source.OnChipRemoved -= DeactiveChipEffects;
            InputManager.Source.Consumable1 += HealPlayer;
        }

        private void InitializeStatManager()
        {
            _currentMaxHealth = _currentBasePlayerStats.MaxHealth; ///
            _currentMovementSpeed = _currentBasePlayerStats.SpeedPower; ///  Colocar datos del savesystem
            _currentDamage = _currentBasePlayerStats.AttackPower;///
            _currentHealth = _currentMaxHealth;
            _isDead = false;

            OnBaseStatsChanged?.Invoke(_currentMaxHealth, _currentMovementSpeed, _currentDamage);
            OnHealthChanges?.Invoke();
        }

        public void AddModifierToPlayer(BonusStatsChip bonusStats)
        {
            _bonusHealth += bonusStats.BonusHealth;
            _bonusMovementSpeed += bonusStats.BonusSpeed;
            _bonusDamage += bonusStats.BonusDamage;

            _currentMaxHealth = _baseHealth + _bonusHealth;
            _currentMovementSpeed = _baseMovementSpeed + _bonusMovementSpeed;
            _currentDamage = _baseDamage + _bonusDamage;
            OnBaseStatsChanged?.Invoke(_currentMaxHealth, _currentMovementSpeed, _currentDamage);

            //float healthPercentage = _currentHealth / (_currentMaxHealth - bonusStats.BonusHealth);
            //_currentHealth = _currentMaxHealth * healthPercentage;
            OnHealthChanges?.Invoke();
        }

        public void SubstractModifierToPlayer(BonusStatsChip bonusStats)
        {
            _bonusHealth -= bonusStats.BonusHealth;
            _bonusMovementSpeed -= bonusStats.BonusSpeed;
            _bonusDamage -= bonusStats.BonusDamage;

            _currentMaxHealth = _baseHealth - _bonusHealth;
            _currentMovementSpeed = _baseMovementSpeed - _bonusMovementSpeed;
            _currentDamage = _baseDamage - _bonusDamage;
            OnBaseStatsChanged?.Invoke(_currentMaxHealth, _currentMovementSpeed, _currentDamage);

            if (_currentHealth > _currentMaxHealth)
            {
                _currentHealth = _currentMaxHealth;
                OnHealthChanges?.Invoke();
            }
        }

        public void TakeDamage(float damage)
        {
            if (_isDead) return;

            _currentHealth -= damage;
            _currentHealth = Mathf.Max(0, _currentHealth);
            
            StartCoroutine(DamageFlashEffect());

            OnDamageRecieved?.Invoke();
            Debug.Log(_currentHealth);

            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        
        private IEnumerator DamageFlashEffect()
        {
            if (playerRenderer != null)
            {
                Color originalColor = playerRenderer.material.color;
                playerRenderer.material.color = Color.red;
            
                yield return new WaitForSeconds(0.1f);
            
                playerRenderer.material.color = originalColor;
            }
        }
        
        private void HealPlayer()
        {
            if (Inventory.Source.GetItemQuantity(_potion) < 1 || _currentHealth == _currentMaxHealth) return;

            Heal(5);
        }

        public void Heal(float healAmount)
        {
            if (_isDead) return;

            _currentHealth += healAmount;
            _currentHealth = Mathf.Min(_currentHealth, _currentMaxHealth);

            OnHealRecieved?.Invoke();
        }

        public void TakeDamage(int damage, bool isMelee)
        {
            TakeDamage(damage);
        }

        public void Die()
        {
            _isDead = true;

            OnPlayerDeath?.Invoke();
            _playerAnim.SetTrigger("Die");
            StartCoroutine(WaitBeforeTeleport(4));
        }

        private IEnumerator WaitBeforeTeleport(float secondsToWait = 3)
        {
            yield return new WaitForSeconds(secondsToWait);
            if (CheckpointSystem.Instance != null)
            {
                CheckpointSystem.Instance.OnPlayerDeath();
            }
        }

        public void Respawn()
        {
            _isDead = false;
            _currentHealth = _currentMaxHealth;
            _playerAnim.SetTrigger("Jump");
            OnHealthChanges?.Invoke();
        }

        public void ActiveChipEffects(Chip chip)
        {
            /*if (chip.ChipData.BonusStatsChip.ActiveHealthBar)
            {
                _healthBar.GetComponent<CanvasGroup>().alpha = 1;
                OnHealthBarActivation?.Invoke(true);
            }

            if (chip.ChipData.BonusStatsChip.ActiveConsumiblesVisualizer)
            {
                _consumiblesUI.GetComponent<CanvasGroup>().alpha = 1;
                OnConsumiblesUIActivation?.Invoke(true);
            }

            if (chip.ChipData.BonusStatsChip.ActiveMeleeAttack)
            {
                _currentBasePlayerStats = _basePlayerStats[0];
                RefreshBaseStats();
                OnHealthChanges?.Invoke();
                OnMeleeChipActivation?.Invoke(true);
            }

            if (chip.ChipData.BonusStatsChip.ActiveRangeAttack)
            {
                _currentBasePlayerStats = _basePlayerStats[1];
                RefreshBaseStats();
                OnHealthChanges?.Invoke();
                OnRangeChipActivation?.Invoke(true);
            }

            if (chip.ChipData.BonusStatsChip.ActiveDash)
            {
                OnDashChipActivation?.Invoke(true);
            }*/
        }

        public void DeactiveChipEffects(Chip chip)
        {
            /*if (chip.ChipData.BonusStatsChip.ActiveHealthBar)
            {
                _healthBar.GetComponent<CanvasGroup>().alpha = 0;
                OnHealthBarActivation?.Invoke(false);
            }

            if (chip.ChipData.BonusStatsChip.ActiveConsumiblesVisualizer)
            {
                _consumiblesUI.GetComponent<CanvasGroup>().alpha = 0;
                OnConsumiblesUIActivation?.Invoke(false);
            }

            if (chip.ChipData.BonusStatsChip.ActiveMeleeAttack)
            {
                OnMeleeChipActivation?.Invoke(false);
            }

            if (chip.ChipData.BonusStatsChip.ActiveRangeAttack)
            {
                OnRangeChipActivation?.Invoke(false);
            }

            if (chip.ChipData.BonusStatsChip.ActiveDash)
            {
                OnDashChipActivation?.Invoke(false);
            }*/
        }

        private void RefreshBaseStats()
        {
            _baseDamage = _currentBasePlayerStats.AttackPower;
            _baseMovementSpeed = _currentBasePlayerStats.SpeedPower;
            _baseHealth = _currentBasePlayerStats.MaxHealth;

            _currentMaxHealth = _baseHealth + _bonusHealth;
            _currentMovementSpeed = _baseMovementSpeed + _bonusMovementSpeed;
            _currentDamage = _baseDamage + _bonusDamage;
            if (_currentHealth > _currentMaxHealth)
            {
                _currentHealth = _currentMaxHealth;
            }
            OnBaseStatsChanged?.Invoke(_currentMaxHealth, _currentMovementSpeed, _currentDamage);

        }
    }
}
