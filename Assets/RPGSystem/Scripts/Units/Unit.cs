using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace RPGSystem
{   
    public abstract class Unit : ScriptableObject
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
            get { return m_unitNickname == string.Empty ? m_unitData.unitName : m_unitNickname; }
        }

        /// <summary>
        /// Quick access for a way to write the Unit's name.
        /// </summary>
        public virtual string displayName
        {
            get
            {
                return m_unitNickname == m_unitData.unitName ? m_unitNickname : unitNickname + " (" + m_unitData.unitName + ")";
            }
        }

        // skill slots
        [SerializeField] protected List<SkillSlot> m_skillSlots = new List<SkillSlot>(GameSettings.MAX_SKILLS_PER_UNIT);

        public List<SkillSlot> skillSlots { get { return m_skillSlots; } }

        // exp/level
        [SerializeField] [Min(0)] protected int m_totalExp;
        [SerializeField] protected int m_expToNextLevel;
        [SerializeField] protected int m_level = 1;
        public int exp { get { return m_totalExp; } }
        public int expToNextLevel { get { return m_expToNextLevel; } }
        public int level { get { return m_level; } }

        /// <summary>
        /// Basic HP stat.
        /// </summary>
        protected int m_currentHP;
        public int currentHP
        {
            get { return m_currentHP; }
            set { m_currentHP = value; }
        }

        public abstract int GetStat(BaseStatName stat);

        // public methods for battle scene
        /// <summary>
        /// Initialises or replaces a SkillSlot in skillSlots with a new Skill in the first empty SkillSlot
        /// or at the specified index.
        /// </summary>
        /// <param name="skill">The Skill to learn.</param>
        /// <param name="skillSlotIndex">Index in skillSlots to overwrite.</param>
        public void LearnSkill(Skill skill, int skillSlotIndex = 0)
        {
            // put the skill into the first empty SkillSlot
            for (int i = 0; i < m_skillSlots.Count; i++)
                if (m_skillSlots[i].skill == null)
                {
                    m_skillSlots[i] = new SkillSlot(skill);
                    return;
                }

            // Otherwise overwrite the SkillSlot at the specified index
            m_skillSlots[skillSlotIndex] = new SkillSlot(skill);
        }

        /// <summary>
        /// Increase the unit's experience by an amount. Returns true when the unit has gained enough experience to level up.
        /// Prevents units that have reached the level cap from gaining experience.
        /// </summary>
        /// <param name="value">Amount of experience for unit to gain.</param>
        /// <returns></returns>
        public void GainExp(int value) // unfinished
        {
            if (m_level < GameSettings.MAX_UNIT_LEVEL)
            {
                Debug.Log(displayName + " gained " + value + " experience-points.");
                int expGained = value;
                m_expToNextLevel -= value;

                // if the unit has gained enough experience to level up, increase the level by the correct amount
                if (expGained - m_expToNextLevel >= 0)
                {
                    int levelsToGain = 0;
                    while (expGained - m_expToNextLevel >= 0)
                    {
                        expGained -= m_expToNextLevel;
                        m_totalExp += m_expToNextLevel;
                        levelsToGain++;
                        m_expToNextLevel = CalculateExpToNextLevel(m_level + levelsToGain);
                    }
                    LevelUp(levelsToGain, expGained);
                }
                // otherwise just update exp totals
                else
                {
                    m_expToNextLevel -= value;
                    m_totalExp += value;
                }
            }
        }

        public void LevelUp(int numberOfLevels = 1, int expRemainder = 0)
        {
            // increase the unit's level
            m_level += numberOfLevels;

            // once level reaches max, no more experience can be gained
            if (m_level >= GameSettings.MAX_UNIT_LEVEL)
            {
                m_level = GameSettings.MAX_UNIT_LEVEL;
                m_expToNextLevel = 0;
                Debug.Log(displayName + " has reached the maximum level!");
                return;
            }

            // calculate the next required amount of experience
            m_expToNextLevel = CalculateExpToNextLevel(m_level) - expRemainder;

            Debug.Log(displayName + " grew to level " + m_level + "!");
        }

        /// <summary>
        /// Define how much exp a unit should be worth here.
        /// </summary>
        /// <returns>The amount of exp a Unit would gain for defeating this Unit.</returns>
        public abstract int GetExpWorth();

        /// <summary>
        /// Returns the amount of experience this Unit would need to Level Up at the specified level.
        /// Override this method to change how your levelling system works.
        /// </summary>
        /// <param name="level">The level to calculate the exp to next level for.</param>
        /// <param name="remainder">The number of remaining experience points after subtracting the </param>
        /// <returns></returns>
        public abstract int CalculateExpToNextLevel(int level, int remainder = 0);
        
        // functional methods
        /// <summary>
        /// Set's the Unit's level to the specified level.
        /// </summary>
        /// <param name="value">Level to set the Unit to.</param>
        public void SetLevel(int value)
        {
            m_totalExp = 0;
            // level cannot exceed max
            m_level = value > GameSettings.MAX_UNIT_LEVEL ? GameSettings.MAX_UNIT_LEVEL : value;
            m_expToNextLevel = 0;
            for (int i = 1; i < value; i++)
                m_totalExp += CalculateExpToNextLevel(i);
            m_expToNextLevel = CalculateExpToNextLevel(m_level);
        }

        /// <summary>
        /// Set's the Unit's total experience to the specified value.
        /// </summary>
        /// <param name="value"></param>
        public void SetTotalExp(int value)
        {
            m_level = 1;
            m_totalExp = value;
            m_expToNextLevel = CalculateExpToNextLevel(m_level);
            int experience = value;
            while (experience > m_expToNextLevel)
            {
                if (m_level == GameSettings.MAX_UNIT_LEVEL)
                    break;
                experience -= m_expToNextLevel;
                GainExp(m_expToNextLevel);
                LevelUp();
            }
        }

        /// <summary>
        /// Cut any null entries from the Unit's SkillSlots.
        /// </summary>
        public virtual void InitialiseForBattle()
        {
            m_skillSlots = m_skillSlots.Where(slot => slot.skill != null).ToList();
        }

        public virtual void ResetUnit()
        {
            m_unitData = null;
            m_unitNickname = string.Empty;
            m_totalExp = 0;
            m_expToNextLevel = 0;
            m_level = 1;
            m_skillSlots = null;
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
                        Debug.LogError(displayName + " does not have a SkillSlot with " + skill.skillName + ".");
                }
            }
            else
                Debug.LogError(displayName + " has no SkillSlots to remove.");
        }
    }
}
