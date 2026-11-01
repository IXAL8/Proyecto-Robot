using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "New Attack Boost", menuName = "PlayerStat/Effects/Attack Boost")]
    public class DummyEffectSO : ChipEffect
    {
        [SerializeField] float _bonusAttack = 10;
        public override void OnPlaced(ChipsClass chip)
        {
            N_PlayerStatsManager.Source.AddAttackBonus(_bonusAttack);
        }

        public override void OnRemoved(ChipsClass chip)
        {
            N_PlayerStatsManager.Source.RemoveAttackBonus(_bonusAttack);
        }
    }
}
