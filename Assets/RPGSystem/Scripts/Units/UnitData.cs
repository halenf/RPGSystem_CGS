using UnityEngine;

namespace RPGSystem
{
    public abstract class UnitData : ScriptableObject
    {
        // unit name
        [SerializeField] protected string m_unitName;
        // values that represent how the unit will grow
        [SerializeField] protected BaseStat[] m_baseStats = new BaseStat[GameSettings.STAT_NAMES.Length];
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
            int numOfStats = GameSettings.STAT_NAMES.Length;
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

