using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    public abstract class UnitData : ScriptableObject
    {
        [Serializable]
        public class BaseStat
        {
            [SerializeField] private BaseStatName m_stat;
            [SerializeField] private int m_value;

            public BaseStatName stat { get { return m_stat; } }
            public int value { get { return m_value; } }
        }

        // unit name
        [SerializeField] protected string m_unitName;
        // values that represent how the unit will grow
        [SerializeField] protected BaseStat[] m_baseStats;

        public string unitName { get { return m_unitName; } }
        public BaseStat[] baseStats { get { return m_baseStats; } }

        private void InitialiseBaseStats()
        {
            m_baseStats = new BaseStat[Enum.GetNames(typeof(BaseStatName)).Length];
        }

        [ContextMenu("Reset")]
        public virtual void ResetUnitData()
        {
            m_unitName = string.Empty;
            InitialiseBaseStats();
        }
    }
}

