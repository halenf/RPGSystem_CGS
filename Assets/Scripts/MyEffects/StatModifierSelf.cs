using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatModifierSelf", menuName = "MyGame/Effects/StatModSelf", order = 1)]
public class StatModifierSelf : Effect
{
    [SerializeField] private BaseStatName m_stat;

    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {
        if (m_stat == BaseStatName.Health)
            return;

        user.ApplyStatModifier(m_stat, m_value);
        Debug.Log(user.displayName + " changed its " + GameSettings.STAT_NAMES[(int)m_stat] + " by " + m_value + " percent!");
    }
}