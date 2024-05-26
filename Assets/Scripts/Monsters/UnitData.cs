using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    public enum UnitBaseStatNames
    {
        Health = 0,
        Strength = 1,
        Fortitude = 2,
        Agility = 3
    }

    public enum UnitLevelCurve
    {
        Slow = 220,
        Medium = 200,
        Fast = 185
    }

    [System.Serializable]
    public class BaseStats
    {
        protected int m_health, m_strength, m_fortitude, m_agility;
        public int health { get { return m_health; } }
        public int strength { get { return m_strength; } }
        public int fortitude { get { return m_fortitude; } }
        public int agility { get { return m_agility; } }
    }

    [CreateAssetMenu(fileName = "UnitData", menuName = "RPGSystem/UnitData", order = 1)]
    public class UnitData : ScriptableObject
    {
        // unit name
        [SerializeField] protected string m_unitName;
        // values that represent how the unit will grow
        [SerializeField] protected BaseStats m_baseStats;
        // represents the amount of experience a unit needs to level up
        [SerializeField] protected UnitLevelCurve m_levelCurve;
        // list of skills a unit can learn as it levels up
        [SerializeField] protected Dictionary<int, Skill> m_levelUpSkills;
        
        public string unitName
        {
            get { return m_unitName; }
        }
        public BaseStats baseStats
        {
            get
            {
                return m_baseStats;
            }
        }
        public UnitLevelCurve levelCurve
        {
            get
            {
                return m_levelCurve;
            }
        }
        public Dictionary<int, Skill> levelUpSkills
        {
            get
            {
                return m_levelUpSkills;
            }
        }
    }
}
