using RPGSystem;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DamageEffect", menuName = "MyGame/Effects/Damage")]
public class DamageEffect : Effect
{
    [SerializeField] private ValueType m_valueType;
    [SerializeField] private EffectPower m_power;

    public override void DoEffect(BattleUnit user, BattleUnit target)
    {       
        int damage = 0;
        switch(m_valueType)
        {
            case ValueType.Value:
                damage = m_value;
                BattleTextLog.Instance.AddLine(target.displayName + " takes " + m_value + " damage!");
                break;
            case ValueType.UserStat:
            case ValueType.TargetStat:
                damage = (int)((int)m_power * (m_valueType == ValueType.UserStat ? user.GetStat(BaseStatName.Strength) : 
                    target.GetStat(BaseStatName.Strength) / (target.GetStat(BaseStatName.Fortitude) + 10.0f)) / 4.0f);
                int targetHP = target.currentHP;
                BattleTextLog.Instance.AddLine(target.displayName + " takes " + damage + " damage!");
                break;
            case ValueType.Percentage:
                damage = (int)(target.GetStat(BaseStatName.Health) / 100.0f * m_value);
                BattleTextLog.Instance.AddLine(target.displayName + " loses " + m_value + "% of its HP (" + damage + " damage)!");
                break;
        }

        target.TakeDamage(damage);

        // if lifesteal is active, heal the user for the damage dealt
        if (user.triggeredEffects.HasFlag(TriggeredEffect.Lifesteal))
        {
            // If damage > targetHP, then there will be overkill damage
            // Don't restore from overkill damage
            int restore;
            if (damage > target.currentHP)
                restore = target.currentHP;
            else
                restore = damage;

            BattleTextLog.Instance.AddLine(user.displayName + " steals " + restore + " HP from " + target.displayName + "!");
            user.RestoreHealth(restore);
        }
    }
}
