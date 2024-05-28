using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "HealEffect", menuName = "Effects/Heal", order = 1)]
public class HealEffect : Effect
{
    [SerializeField] private ValueType m_valueType;
    [SerializeField] private EffectPower m_power;

    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {
        foreach (BattleUnit target in targets)
        {
            int restore = 0;

            switch (m_valueType)
            {
                case ValueType.Value:
                    restore = m_value;
                    Debug.Log(target.displayName + " restores " + restore + " HP!");
                    break;
                case ValueType.UserStat:
                case ValueType.TargetStat:
                    restore = (int)((int)m_power * user.unit.maxHP * 0.08f);
                    Debug.Log(target.displayName + " restores " + restore + " HP!");
                    break;
                case ValueType.Percentage:
                    restore = (int)(target.maxHP / 100.0f * m_value);
                    Debug.Log(target.displayName + " restores " + m_value + "% of its HP (" + restore + " HP)!");
                    break;
            }

            // don't restore HP over maximum
            if (target.currentHP + restore > target.maxHP)
                restore = target.maxHP - target.currentHP;

            target.RestoreHealth(restore);
        }
    }
}