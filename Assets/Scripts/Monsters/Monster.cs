using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    [Serializable]
    public class SkillSlot
    {
        public SkillSlot() { }
        public SkillSlot(Skill skill)
        {
            m_skill = skill;
            m_turnTimer = skill.turnTimer;
        }

        [SerializeField] private Skill m_skill;
        [SerializeField] [Min(0)] private int m_turnTimer;

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
    
    [CreateAssetMenu(fileName = "Monster", menuName = "RPGSystem/Monsters/Monster", order = 1)]
    public class Monster : ScriptableObject
    {
        // monster data
        [SerializeField] private MonsterData m_monsterData;
        public MonsterData monsterData
        {
            get { return m_monsterData; }
        }

        [SerializeField] private string m_monsterName;
        public string monsterName
        {
            get { return m_monsterName; }
        }

        // skill slots
        [SerializeField] private List<SkillSlot> m_skillSlots = new List<SkillSlot>();

        public List<SkillSlot> skillSlots
        {
            get
            {
                return m_skillSlots;
            }
        }

        // exp/m_level
        [SerializeField] [Min(0)] private int m_totalExp;
        public int exp
        {
            get { return m_totalExp; }
        }
        [SerializeField] private int m_expToNextLevel;
        public int expToNextLevel
        {
            get { return m_expToNextLevel; }
        }
        [SerializeField] [Range(1, 100)] private int m_level = 1;
        public int level
        {
            get { return m_level; }
        }

        // base stat accessors
        public int health
        {
            get
            {
                return monsterData.baseStats.health;
            }
        }

        public int strength
        {
            get
            {
                return monsterData.baseStats.strength;
            }
        }

        public int fortitude
        {
            get
            {
                return monsterData.baseStats.fortitude;
            }
        }

        public int agility
        {
            get
            {
                return monsterData.baseStats.agility;
            }
        }

        // volatile stat accessors
        public int maxHP
        {
            get
            {
                return (int)(health * Mathf.Pow(1000 * (m_level + 1), 0.4f));
            }
        }

        public int attack
        {
            get
            {
                return (int)(30 * strength * (m_level + 1) / 100.0f + 10);
            }
        }
        public int defence
        {
            get
            {
                return (int)(30 * fortitude * (m_level + 1) / 100.0f + 10);
            }
        }
        public int speed
        {
            get
            {
                return (int)(30 * agility * (m_level + 1)/ 100.0f + 10);
            }
        }

        // public methods for battle scene
        /// <summary>
        /// Increase the monster's experience by an amount. Returns true when the monster has gained enough experience to level up.
        /// A monster cannot gain experience once they reach the level cap.
        /// </summary>
        /// <param name="value">Amount of experience for monster to gain.</param>
        /// <returns></returns>
        public bool GainExp(int value)
        {
            if (m_level < 100)
            {
                m_expToNextLevel -= value;
                if (m_expToNextLevel <= 0)
                    return true;
            }
            return false;
        }

        public int GetExpWorth()
        {
            throw new NotImplementedException("The GetExpWorth method has not been implemented.");
        }

        public void LevelUp(int remainder)
        {
            m_level++;
            if (m_level == 100)
                remainder = 0;
            m_expToNextLevel = GetExpToNextLevel(m_level) - remainder;
        }

        public int GetExpToNextLevel(int level)
        {
            if (level == 100)
                return 0;
            return (int)Mathf.Pow(level, 2.1f) + (int)m_monsterData.levelCurve * level;
        }

        // functional methods
        public void SetLevel(int value)
        {
            m_totalExp = 0;
            m_level = value;
            m_expToNextLevel = 0;
            for (int i = 0; i < value - 1; i++)
                m_totalExp += GetExpToNextLevel(i + 1);
            m_expToNextLevel = GetExpToNextLevel(m_level);
        }

        public void SetTotalExp(int value)
        {
            m_level = 1;
            m_totalExp = value;
            m_expToNextLevel = GetExpToNextLevel(m_level);
            int experience = value;
            while (experience > m_expToNextLevel)
            {
                experience -= m_expToNextLevel;
                GainExp(m_expToNextLevel);
                LevelUp(-m_expToNextLevel);
            }
        }

        public void ResetMonsterData()
        {
            m_monsterData = null;
            m_monsterName = string.Empty;
            m_totalExp = 0;
            m_expToNextLevel = 0;
            m_level = 1;
            m_skillSlots = null;

            //this = new Monster();
        }

        public void AddSkillSlot(Skill skill = null, int turnTimer = 0)
        {
            if (skill == null)
                // add a new empty skill slot
                m_skillSlots.Add(new SkillSlot());
            else
                // add a skill slot with the predefined values
                m_skillSlots.Add(new SkillSlot(skill));
        }

        public void RemoveSkillSlot(Skill skill = null)
        {
            if (m_skillSlots.Count > 0)
            {
                if (skill == null)
                    // remove the last skill slot in the list
                    m_skillSlots.RemoveAt(m_skillSlots.Count - 1);
                else
                {
                    // removes all skill slots that have a matching skill
                    // a monster shouldn't be able to have two of the same skill so it should only remove one skill slot,
                    // if it has that skill
                    m_skillSlots.RemoveAll(skillSlot => skillSlot.skill == skill);
                }
            }
            else
                throw new IndexOutOfRangeException(name + " has no skill slots to remove.");
        }
    }
}
