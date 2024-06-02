using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyUnit", menuName = "Unit", order = 1)]
public class MyUnit : Unit
{
    private int HealthStatFormula()
    {
        return (int)(m_unitData.baseStats[(int)BaseStatName.Health].value * Mathf.Pow(1000 * (m_level + 1), 0.4f));
    }
    
    private int BasicStatFormula(BaseStatName stat)
    {
        if (stat != BaseStatName.Health)
            return (int)(30 * m_unitData.baseStats[(int)stat].value * (m_level + 1) / 100.0f + 10);
        else
            return HealthStatFormula();
    }

    public override int GetExpWorth()
    {
        return (int)(CalculateExpToNextLevel(m_level) / 4.0f);
    }

    /// <summary>
    /// Quick EXP to next level using the Unit's current level.
    /// </summary>
    /// <returns>Exp required for this Unit to reach its next level.</returns>
    public int CalculateExpToNextLevel()
    {
        return CalculateExpToNextLevel(m_level);
    }

    public override int CalculateExpToNextLevel(int level, int remainder = 0)
    {
        if (level == GameSettings.MAX_UNIT_LEVEL)
            return 0;
        return (int)Mathf.Pow(level, 2.1f) + (int)(m_unitData as MyUnitData).levelCurve * level - remainder;
    }

    public override int GetStat(BaseStatName stat)
    {
        return BasicStatFormula(stat);
    }

    [SerializeField] private int m_balls;

    public int maxHP { get { return HealthStatFormula(); } }
    public int attack { get { return BasicStatFormula(BaseStatName.Strength); } }
    public int defence { get { return BasicStatFormula(BaseStatName.Fortitude); } }
    public int speed { get { return BasicStatFormula(BaseStatName.Agility); } }
}
