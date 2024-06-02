using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override void ResetBattleUnit()
    {
        base.ResetBattleUnit();

        // reset HP
        currentHP = maxHP;
    }
}
