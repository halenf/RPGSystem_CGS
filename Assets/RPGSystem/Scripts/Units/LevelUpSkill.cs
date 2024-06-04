using System;
using UnityEngine;

namespace RPGSystem
{
    [Serializable]
    public class LevelUpSkill
    {
        [SerializeField] private int m_level;
        [SerializeField] private Skill m_skill;

        public int level { get { return m_level; } }
        public Skill skill { get { return m_skill; } }
    }
}