using System;
using UnityEngine;

namespace RPGSystem
{
    [Serializable]
    public class BaseStat
    {
        public BaseStat(BaseStatName stat, int value = 0)
        {
            m_stat = stat;
            m_value = 0;
        }

        [SerializeField] private BaseStatName m_stat;
        [SerializeField] private int m_value;

        public BaseStatName stat { get { return m_stat; } }
        public int value { get { return m_value; } }
    }
}
