using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "StatEffect", menuName = "Effects/StatModifier", order = 1)]
public class StatModifierEffect : Effect
{
    // note: HP cannot be boosted via this method
    [SerializeField] private BaseStatName m_baseStat;
    public BaseStatName baseStat
    {
        get { return m_baseStat; }
    }

    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {
        if (m_baseStat == BaseStatName.Health)
            return;

        foreach (BattleUnit target in targets)
        {
            target.ApplyStatModifier(m_baseStat, m_value);
            Debug.Log(target.displayName + "'s " + m_baseStat.ToString() + " is " + (m_value > 0 ? "raised" : "dropped") + " by " + m_value + "%!");
        }
    }
}
