using RPGSystem;
using System;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MyBattleUnit : BattleUnit
{
    public MyBattleUnit() { }
    public MyBattleUnit(MyUnit unit, BattleUnitID id)
    {
        m_unit = unit;
        m_battleID = id;
        currentHP = maxHP;
    }

    // stats
    public int maxHP
    {
        get { return m_unit.GetStat(BaseStatName.Health); }
    }
    public int attack
    {
        get { return m_unit.GetStat(BaseStatName.Strength); }
    }
    public int defence
    {
        get { return m_unit.GetStat(BaseStatName.Fortitude); }
    }
    public int speed
    {
        get { return m_unit.GetStat(BaseStatName.Agility); }
    }

    public bool canActThisTurn
    {
        get
        {
            return !m_unit.skillSlots.TrueForAll(slot => slot.turnTimer == 0) || !m_triggeredEffects.HasFlag(TriggeredEffect.Stun);
        }
    }

    public int GetStat(string stat)
    {
        if (GameSettings.STAT_NAMES.Contains(stat))
        {
            return GetStat((BaseStatName)Array.IndexOf(GameSettings.STAT_NAMES, stat));
        }
        else
            Debug.LogError("The Stat \"" + stat + "\" does not exist!");
        return 0;
    }

    public override void ResetBattleUnit()
    {
        base.ResetBattleUnit();

        // reset HP
        currentHP = maxHP;
    }
}
