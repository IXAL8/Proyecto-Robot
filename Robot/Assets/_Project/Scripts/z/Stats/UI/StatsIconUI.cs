using TMPro;
using UnityEngine;

namespace Robot
{
    public class StatsIconUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _maxHealthValue;
        [SerializeField] private TextMeshProUGUI _attackValue;
        [SerializeField] private TextMeshProUGUI _moveSpeedValue;

        private void OnEnable()
        {
            RefreshValues();
            N_PlayerStatsManager.Source.OnStatsChanged += RefreshValues;
        }

        private void OnDisable()
        {
            N_PlayerStatsManager.Source.OnStatsChanged -= RefreshValues;
        }

        private void RefreshValues()
        {
            _maxHealthValue.text = (N_PlayerStatsManager.Source.MaxHealth/10).ToString();
            _attackValue.text = (N_PlayerStatsManager.Source.Attack/10).ToString();
            _moveSpeedValue.text = (N_PlayerStatsManager.Source.MoveSpeed).ToString();
        }
    }
}
