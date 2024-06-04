using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGSystem
{
    public abstract class Unit : ScriptableObject
    {
        /// <summary>
        /// The UnitData this Unit is built from.
        /// </summary>
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

        /// <summary>
        /// Returns the stat corresponding to the supplied BaseStat.
        /// </summary>
        public abstract int GetStat(BaseStatName stat);

        /// <param name="level"></param>
        /// <returns>The Skills this Unit learns at the specified level.</returns>
        public Skill[] GetSkillsForLevel(int level)
        {
            return m_unitData.levelUpSkills.Where(levelUpSkill => levelUpSkill.level == level).Select(levelUpSkill => levelUpSkill.skill).ToArray();
        }

        /// <summary>
        /// Initialises the Unit with the Skills it should know at its current level.
        /// </summary>
        public void InitialiseSkillSlots()
        {
            // empty m_skillSlots
            m_skillSlots = new List<SkillSlot>();

            if (m_unitData.levelUpSkills.Length == 0)
            {
                Debug.LogError(m_unitData.unitName + " UnitData has no level up skills!");
                return;
            }

            // get the skills for the unit's level
            LevelUpSkill[] skillsForLevel = m_unitData.levelUpSkills.Where(levelUpSkill => levelUpSkill.level <= m_level).
                OrderByDescending(levelUpSkill => levelUpSkill.level).ToArray();

            // calc the number of skills the unit should learn
            int numOfSkills = GameSettings.MAX_SKILLS_PER_UNIT >= skillsForLevel.Length ? skillsForLevel.Length : GameSettings.MAX_SKILLS_PER_UNIT;

            // add the skills to m_skillSlots
            for (int i = 0; i < numOfSkills; i++)
                AddSkillSlot(skillsForLevel[i].skill);
        }

        /// <summary>
        /// Initialises or replaces a SkillSlot in skillSlots with a new Skill in the first empty SkillSlot
        /// or at the specified index.
        /// </summary>
        /// <param name="skill">The Skill to learn.</param>
        /// <param name="skillSlotIndex">Index of the SkillSlot to overwrite.</param>
        public void LearnSkill(Skill skill, int skillSlotIndex)
        {
            // put the skill into the first empty SkillSlot, the designated SkillSlot,
            // or add to the end if the list is not at the max yet
            for (int i = 0; i < GameSettings.MAX_SKILLS_PER_UNIT; i++)
            {
                if (m_skillSlots.ElementAtOrDefault(i) != null)
                {
                    if (m_skillSlots[i].skill == null || i == skillSlotIndex)
                    {
                        m_skillSlots[i] = new SkillSlot(skill);
                        return;
                    }
                }
                else
                {
                    m_skillSlots.Add(new SkillSlot(skill));
                    return;
                }
            }
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
            m_expToNextLevel = CalculateExpToNextLevel(m_level, expRemainder);

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

            // level cannot exceed max or be less than 1
            if (value < 1)
                m_level = 1;
            else
                m_level = value > GameSettings.MAX_UNIT_LEVEL ? GameSettings.MAX_UNIT_LEVEL : value;
            m_expToNextLevel = 0;
            for (int i = 1; i < m_level; i++)
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
            m_totalExp = 0;
            m_expToNextLevel = CalculateExpToNextLevel(m_level);
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
            m_totalExp += experience;
            m_expToNextLevel -= experience;
        }

        /// <summary>
        /// Prepare the Unit for a battle. Override to add your own functionality.
        /// </summary>
        public virtual void InitialiseForBattle()
        {
            //Cut any null entries from the Unit's SkillSlots.
            m_skillSlots = m_skillSlots.Where(slot => slot.skill != null).ToList();
            if (m_skillSlots.Count == 0)
                InitialiseSkillSlots();
        }

        public virtual void ResetUnit()
        {
            m_unitData = null;
            m_unitNickname = string.Empty;
            m_totalExp = 0;
            m_expToNextLevel = 0;
            m_level = 1;
            m_skillSlots = new List<SkillSlot>();
        }

        public void AddSkillSlot(Skill skill = null)
        {
            if (m_skillSlots.Count == GameSettings.MAX_SKILLS_PER_UNIT)
            {
                Debug.LogError(displayName + " already has max (" + GameSettings.MAX_SKILLS_PER_UNIT + ") Skills.");
                return;
            }

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
