using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit", order = 1)]
public class MyUnit : Unit
{
    private int BasicStatFormula(int baseStat)
    {
        return (int)(30 * baseStat * (m_level + 1) / 100.0f + 10);
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
        return (int)Mathf.Pow(level, 2.1f) + (int)m_unitData.levelCurve * level - remainder;
    }
}
