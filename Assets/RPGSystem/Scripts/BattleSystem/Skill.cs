using System;
using UnityEngine;

namespace RPGSystem
{
    [CreateAssetMenu(fileName = "Skill", menuName = "RPGSystem/Skill", order = 0)]
    public class Skill : ScriptableObject
    {
        [Serializable]
        public enum TargetType
        {
            SingleEnemy = 0,
            Self = 1,
            AdjacentEnemies = 2,
            AllEnemies = 4,
            SingleParty = 8,
            WholePartyButSelf = 16,
            WholeParty = 32,
            EveryoneButSelf = 64,
            Everyone = 128
        }

        [Serializable]
        public enum SkillPriority
        {
            AlwaysLast = -3,
            None = 0,
            AlwaysFirst = 3
        }
        
        /// <summary>
        /// The name of the Skill.
        /// </summary>
        [SerializeField] protected string m_skillName;
        /// <summary>
        /// The default length of the Skill's cooldown.
        /// </summary>
        [SerializeField] protected int m_turnTimer;
        /// <summary>
        /// What the Skill can target.
        /// </summary>
        [SerializeField] protected TargetType m_targets;
        /// <summary>
        /// Turn order priority.
        /// </summary>
        [SerializeField] protected SkillPriority m_priority;
        /// <summary>
        /// The Skill's Effects trigger in the order set here.
        /// </summary>
        [SerializeField] protected Effect[] m_effects;

        public string skillName { get { return m_skillName; } }
        public int turnTimer { get { return m_turnTimer; } }
        public TargetType targets { get { return m_targets; } }
        public Effect[] effects { get { return m_effects; } }
    }

    [Serializable]
    public class SkillSlot
    {
        public SkillSlot() { }
        public SkillSlot(Skill skill)
        {
            m_skill = skill;
            m_turnTimer = skill.turnTimer;
        }

        [SerializeField] protected Skill m_skill;
        [SerializeField][Min(0)] protected int m_turnTimer;

        public Skill skill
        {
            get { return m_skill; }
        }
        public int turnTimer
        {
            get { return m_turnTimer; }
        }

        public void ChangeTurnTimer(int value)
        {
            m_turnTimer += value;
            if (m_turnTimer < 0)
                m_turnTimer = 0;
        }

        public void ResetTimer()
        {
            m_turnTimer = 0;
        }

        public void ClearSlot()
        {
            m_skill = null;
            m_turnTimer = 0;
        }
    }
}
