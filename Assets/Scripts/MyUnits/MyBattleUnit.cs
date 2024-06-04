using RPGSystem;

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
        get { return (int)(m_unit.GetStat(BaseStatName.Strength) * m_statModifiers[BaseStatName.Strength]); }
    }
    public int defence
    {
        get { return (int)(m_unit.GetStat(BaseStatName.Fortitude) * m_statModifiers[BaseStatName.Fortitude]); }
    }
    public int speed
    {
        get { return (int)(m_unit.GetStat(BaseStatName.Agility) * m_statModifiers[BaseStatName.Agility]); }
    }

    public override void ResetBattleUnit()
    {
        base.ResetBattleUnit();

        // reset HP
        currentHP = maxHP;
    }
}
