using System;
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

        [SerializeField] protected Skill m_skill;
        [SerializeField] [Min(0)] protected int m_turnTimer;

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
    
    [CreateAssetMenu(fileName = "Unit", menuName = "RPGSystem/Unit", order = 1)]
    public class Unit : ScriptableObject
    {
        // unit data
        [SerializeField] protected UnitData m_unitData;
        public UnitData unitData
        {
            get { return m_unitData; }
        }

        [SerializeField] protected string m_unitNickname;
        public string unitNickname
        {
            get { return m_unitNickname; }
        }

        // skill slots
        [SerializeField] protected List<SkillSlot> m_skillSlots = new List<SkillSlot>();

        public List<SkillSlot> skillSlots
        {
            get
            {
                return m_skillSlots;
            }
        }

        // exp/m_level
        [SerializeField] [Min(0)] protected int m_totalExp;
        public int exp
        {
            get { return m_totalExp; }
        }
        [SerializeField] protected int m_expToNextLevel;
        public int expToNextLevel
        {
            get { return m_expToNextLevel; }
        }
        [SerializeField] protected int m_level = 1;
        public int level
        {
            get { return m_level; }
        }

        // volatile stat accessors
        protected int m_currentHP;
        public int currentHP
        { 
            get { return m_currentHP; } 
            set { m_currentHP = value; }
        }
        public int maxHP
        {
            get
            {
                return (int)(m_unitData.baseStats.health * Mathf.Pow(1000 * (m_level + 1), 0.4f));
            }
        }

        public int attack
        {
            get
            {
                return (int)(30 * m_unitData.baseStats.strength * (m_level + 1) / 100.0f + 10);
            }
        }
        public int defence
        {
            get
            {
                return (int)(30 * m_unitData.baseStats.fortitude * (m_level + 1) / 100.0f + 10);
            }
        }
        public int speed
        {
            get
            {
                return (int)(30 * m_unitData.baseStats.agility * (m_level + 1)/ 100.0f + 10);
            }
        }

        // public methods for battle scene
        /// <summary>
        /// Increase the unit's experience by an amount. Returns true when the unit has gained enough experience to level up.
        /// Prevents units that have reached the level cap from gaining experience.
        /// </summary>
        /// <param name="value">Amount of experience for unit to gain.</param>
        /// <returns></returns>
        public bool GainExp(int value) // unfinished
        {
            if (m_level < GameSettings.MAX_UNIT_LEVEL)
            {
                m_expToNextLevel -= value;

                // if the unit did not gain enough experience to level up
                // it is safe to add the exp to the total
                if (m_expToNextLevel > 0)
                    m_totalExp += value;

                return true;
            }
            return false;
        }

        public int GetExpWorth()
        {
            throw new NotImplementedException("The GetExpWorth method has not been implemented.");
        }

        public void LevelUp()
        {
            // increase the unit's level
            m_level++;

            // once level reaches max, no more experience can be gained
            if (m_level == GameSettings.MAX_UNIT_LEVEL)
            {   
                m_expToNextLevel = 0;
                    return;
            }

            // calculate the next required amount of experience
            m_expToNextLevel = GetExpToNextLevel(m_level);
        }

        public int GetExpToNextLevel(int level)
        {
            if (level == GameSettings.MAX_UNIT_LEVEL)
                return 0;
            return (int)Mathf.Pow(level, 2.1f) + (int)m_unitData.levelCurve * level;
        }

        // functional methods
        public void SetLevel(int value)
        {
            m_totalExp = 0;
            // level cannot exceed max
            m_level = value > GameSettings.MAX_UNIT_LEVEL ? 100 : value;
            m_expToNextLevel = 0;
            for (int i = 0; i < value - 1; i++)
                m_totalExp += GetExpToNextLevel(i + 1);
            m_expToNextLevel = GetExpToNextLevel(m_level);
        }

        public void SetTotalExp(int value)
        {
            m_level = 1;
            m_totalExp = 0;
            m_expToNextLevel = GetExpToNextLevel(m_level);
            int experience = value;
            while (experience > m_expToNextLevel)
            {
                if (m_level == GameSettings.MAX_UNIT_LEVEL)
                    break;
                experience -= m_expToNextLevel;
                m_totalExp += m_expToNextLevel;
                GainExp(m_expToNextLevel);
                LevelUp();
            }
        }

        public void ResetUnitData()
        {
            m_unitData = null;
            m_unitNickname = string.Empty;
            m_totalExp = 0;
            m_expToNextLevel = 0;
            m_level = 1;
            m_skillSlots = null;

            //this = new Unit();
        }

        public void AddSkillSlot(Skill skill = null)
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
                    // a unit shouldn't be able to have two of the same skill so it should only remove one skill slot,
                    // if it has that skill
                    if (m_skillSlots.RemoveAll(skillSlot => skillSlot.skill == skill) == 0)
                        Debug.LogError(m_unitNickname + "(" + m_unitData.unitName + ") does not have a SkillSlot with " + skill.skillName + ".");
                }
            }
            else
                Debug.LogError(m_unitNickname + "(" + m_unitData.unitName + ") has no SkillSlots to remove.");
        }
    }
}
