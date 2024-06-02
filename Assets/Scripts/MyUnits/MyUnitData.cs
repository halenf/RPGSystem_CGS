using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyUnitData", menuName = "UnitData", order = 1)]
public class MyUnitData : UnitData
{
    public enum UnitLevelCurve
    {
        Slow = 220,
        Medium = 200,
        Fast = 185
    }

    [System.Serializable]
    public class LevelUpSkill
    {
        [SerializeField] private int m_level;
        [SerializeField] private Skill m_skill;

        public int level { get { return m_level; } }
        public Skill skill { get { return m_skill; } }
    }

    // The value that determines how the unit grows
    [SerializeField] private UnitLevelCurve m_levelCurve;
    // list of skills a unit can learn as it levels up
    [SerializeField] protected LevelUpSkill[] m_levelUpSkills;

    public UnitLevelCurve levelCurve { get { return m_levelCurve; } }
    public LevelUpSkill[] levelUpSkills { get { return m_levelUpSkills; } }
}
