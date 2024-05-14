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
        public SkillSlot(Skill skill, int turnTimer)
        {
            m_skill = skill;
            m_turnTimer = turnTimer;
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
            set
            {
                if (value >= 0)
                    m_turnTimer = value;
            }
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

    [Serializable]
    public class StatusSlot
    {
        public StatusSlot() { }
        public StatusSlot(Status status, int turnTimer)
        {
            m_status = status;
            m_turnTimer = turnTimer;
        }

        [SerializeField] private Status m_status;
        [SerializeField] [Min(0)] private int m_turnTimer;

        public Status status
        {
            get { return m_status; }
        }
        public int turnTimer
        {
            get { return m_turnTimer; }
            set
            {
                if (value >= 0)
                    m_turnTimer = value;
            }
        }

        public void OnApply(Monster target)
        {
            foreach (SkillStatusEffect effect in m_status.onApply)
            {
                effect.Effect(m_status.user, new Monster[] { target });
            }
        }

        public void OnTurnEnd(Monster target)
        {
            foreach (SkillStatusEffect effect in m_status.onTurnEnd)
            {
                effect.Effect(m_status.user, new Monster[] { target });
            }
        }

        public void OnClear(Monster target)
        {
            // if clear effects on this target should not fail
            if ((target.triggeredEffects & TriggeredEffect.FailStatusClearEffects) == 0)
            {
                foreach (SkillStatusEffect effect in m_status.onClear)
                {
                    effect.Effect(m_status.user, new Monster[] { target });
                }
            }
            else
                target.ToggleTriggeredEffect(TriggeredEffect.FailStatusClearEffects);
        }

        public void ClearSlot()
        {
            m_status = null;
            m_turnTimer = 0;
        }
    }

    [Serializable]
    [Flags] public enum TriggeredEffect
    {
        None = 0,
        FailNextSkillEffect = 1,
        FailStatusClearEffects = 2, // Implemented
        DamageOnSkillUse = 4,
        DebuffImmunity = 8, // Implemented
        Lifesteal = 16 // Implemented
    }
    
    [CreateAssetMenu(fileName = "Monster", menuName = "RPGSystem_SO/Monsters/Monster", order = 1)]
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

        // skill and status
        [SerializeField] private List<SkillSlot> m_skillSlots = new List<SkillSlot>();
        [SerializeField] private List<StatusSlot> m_statusSlots = new List<StatusSlot>();

        public List<SkillSlot> skillSlots
        {
            get
            {
                return m_skillSlots;
            }
        }
        public List<StatusSlot> statusSlots
        {
            get
            {
                return m_statusSlots;
            }
        }

        // tracks stat modifiers from buffs
        [SerializeField] [Min(0)] private float[] m_statModifiers = new float[4];

        // bitwise enum for effects with triggers
        [SerializeField] private TriggeredEffect m_triggeredEffects;
        public TriggeredEffect triggeredEffects
        {
            get
            {
                return m_triggeredEffects;
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
        private int m_currentHP;
        public int maxHP
        {
            get
            {
                return (int)(health * Mathf.Pow(1000 * (m_level + 1), 0.4f));
            }
        }
        public int currentHP
        {
            get
            {
                return m_currentHP;
            }
        }
        public int attack
        {
            get
            {
                return (int)((30 * strength * (m_level + 1) / 100 + 10) * m_statModifiers[(int)MonsterBaseStats.Strength]);
            }
        }
        public int defence
        {
            get
            {
                return (int)((30 * fortitude * (m_level + 1) / 100 + 10) * m_statModifiers[(int)MonsterBaseStats.Fortitude]);
            }
        }
        public int speed
        {
            get
            {
                return (int)((30 * agility * (m_level + 1)/ 100 + 10) * m_statModifiers[(int)MonsterBaseStats.Agility]);
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

        public void BattleResetMonster()
        {
            // clear stat buffs/debuffs
            for (int i = 1; i < m_statModifiers.Length; i++)
                m_statModifiers[i] = 1;

            // reset HP
            m_currentHP = maxHP;
            
            // reset all skill cooldowns
            foreach (SkillSlot skillSlot in m_skillSlots)
            {
                skillSlot.ResetTimer();
            }

            // clear statuses
            m_statusSlots.Clear();

            // clear triggered effects
            m_triggeredEffects = 0;
        }

        // public methods for skill effects
        public void TakeDamage(int damage)
        {
            // reduce HP
            m_currentHP -= damage;

            if (m_currentHP <= 0)
            {
                // kill TODO
            }
        }

        public void RestoreHealth(int value)
        {
            // increase HP, don't allow monster to have HP over the maximum
            m_currentHP += value;

            if (m_currentHP > maxHP)
                m_currentHP = maxHP;
        }

        public void ChangeStat(MonsterBaseStats baseStat, int value)
        {
            // apply percentage multiplier to specified stat (compounding with other boosts)
            // note: negative values represent a debuff

            // turns value into a 1.X multiplier where X is value
            // or 1 / 1.X value, if debuff
            float _value;
            if (value < 0)
                _value = -1.0f / (value / 100.0f + 1); // make positive, get reciprocal
            else
                _value = value / 100.0f + 1;
            
            m_statModifiers[(int)baseStat] *= _value;
        }

        public void GainStatus(Status status)
        {
            // if target does not have debuff immunity
            if ((m_triggeredEffects & TriggeredEffect.DebuffImmunity) == 0)
            {
                // add new status to status list, then call its on apply affects
                m_statusSlots.Add(new StatusSlot(status, status.turnTimer));
                m_statusSlots.Last().OnApply(this);
            }
            // if target does have debuff immunity, then don't apply the status
        }

        public void ChangeSkillCooldown(int index, int value)
        {
            // increase/decrease the turn timer of the skill at the indicated index
            m_skillSlots[index].turnTimer += value;
        }

        public void ChangeStatusTimer(int index, int value)
        {
            // increase/decrease the turn timer of the status at the indicated index
            m_statusSlots[index].turnTimer += value;
        }

        public void ToggleTriggeredEffect(TriggeredEffect effect)
        {
            m_triggeredEffects ^= effect;
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

        [ContextMenu("Reset Monster Data")]

        public void ResetMonster()
        {
            m_monsterData = null;
            m_monsterName = string.Empty;
            m_skillSlots = null;
            m_statusSlots = null;
            m_statModifiers = null;
            m_triggeredEffects = 0;
            m_totalExp = 0;
            m_currentHP = 0;

            //this = new Monster();
        }

        public void AddSkillSlot(Skill skill = null, int turnTimer = 0)
        {
            if (skill == null)
                // add a new empty skill slot
                m_skillSlots.Add(new SkillSlot());
            else
                // add a skill slot with the predefined values
                m_skillSlots.Add(new SkillSlot(skill, turnTimer));
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

        public void AddStatusSlot(Status status = null, int turnTimer = 0)
        {
            if (status == null)
                // add a new empty status slot
                m_statusSlots.Add(new StatusSlot());
            else
                // add a status slot with the predefined values
                m_statusSlots.Add(new StatusSlot(status, turnTimer));
        }

        public void RemoveStatusSlot(Status status = null)
        {
            if (m_statusSlots.Count > 0)
            {
                if (status == null)
                    // remove the last status slot in the list
                    m_statusSlots.RemoveAt(m_statusSlots.Count - 1);
                else
                    // removes all status slots that have a matching status
                    // a monster shouldn't be able to have two of the same status so it should only remove one status slot,
                    // if it has that status
                    m_statusSlots.RemoveAll(statusSlot => statusSlot.status == status);
            }
            else
                throw new IndexOutOfRangeException(name + " has no status slots to remove.");
        }
    }
}
