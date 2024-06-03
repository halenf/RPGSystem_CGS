using System;
using UnityEngine;

namespace RPGSystem
{
    public abstract class UnitData : ScriptableObject
    {
        [Serializable]
        public class BaseStat
        {
            public BaseStat(BaseStatName stat)
            {
                m_stat = stat;
                m_value = 0;
            }

            [SerializeField] private BaseStatName m_stat;
            [SerializeField] private int m_value;

            public BaseStatName stat { get { return m_stat; } }
            public int value { get { return m_value; } }
        }

        [System.Serializable]
        public class LevelUpSkill
        {
            [SerializeField] private int m_level;
            [SerializeField] private Skill m_skill;

            public int level { get { return m_level; } }
            public Skill skill { get { return m_skill; } }
        }

        // unit name
        [SerializeField] protected string m_unitName;
        // values that represent how the unit will grow
        [SerializeField] protected BaseStat[] m_baseStats = new BaseStat[Enum.GetNames(typeof(BaseStatName)).Length];
        // array of skills a unit can learn as it levels up
        [SerializeField] protected LevelUpSkill[] m_levelUpSkills;
        public string unitName { get { return m_unitName; } }
        public BaseStat[] baseStats { get { return m_baseStats; } }
        public LevelUpSkill[] levelUpSkills { get { return m_levelUpSkills; } }

        protected virtual void Awake()
        {
            InitialiseBaseStats();
        }

        private void InitialiseBaseStats()
        {
            int numOfStats = Enum.GetNames(typeof(BaseStatName)).Length;
            m_baseStats = new BaseStat[numOfStats];
            for (int i = 0; i < numOfStats; i++)
            {
                m_baseStats[i] = new BaseStat((BaseStatName)i);
            }
        }

        [ContextMenu("Reset")]
        public virtual void ResetUnitData()
        {
            m_unitName = string.Empty;
            InitialiseBaseStats();
        }
    }
}

