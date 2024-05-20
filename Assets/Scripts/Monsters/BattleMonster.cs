using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace RPGSystem
{
    [Serializable]
    public class StatusSlot
    {
        /// <summary>
        /// Create an empty status slot.
        /// </summary>
        public StatusSlot() { }
        /// <summary>
        /// Create a status slot with a status.
        /// </summary>
        /// <param name="status">The status.</param>
        public StatusSlot(Status status)
        {
            m_status = status;
            m_turnTimer = status.turnTimer;
        }

        [SerializeField] protected Status m_status;
        [SerializeField][Min(0)] protected int m_turnTimer;

        public Status status
        {
            get { return m_status; }
        }
        public int turnTimer
        {
            get { return m_turnTimer; }
        }

        /// <summary>
        /// Activates the on apply effects of the status.
        /// </summary>
        /// <param name="target"></param>
        public void OnApply(BattleMonster target)
        {
            foreach (SkillStatusEffect effect in m_status.onApply)
            {
                effect.Effect(m_status.user, new BattleMonster[] { target });
            }
        }

        /// <summary>
        /// Activates the end of turn effects of the status.
        /// </summary>
        /// <param name="target"></param>
        public void OnTurnEnd(BattleMonster target)
        {
            foreach (SkillStatusEffect effect in m_status.onTurnEnd)
            {
                effect.Effect(m_status.user, new BattleMonster[] { target });
            }
        }

        /// <summary>
        /// Activates the on clear effects of the status.
        /// </summary>
        /// <param name="target"></param>
        public void OnClear(BattleMonster target)
        {
            // if clear effects on this target should not fail
            if ((target.triggeredEffects & TriggeredEffect.FailStatusClearEffects) == 0)
            {
                foreach (SkillStatusEffect effect in m_status.onClear)
                {
                    effect.Effect(m_status.user, new BattleMonster[] { target });
                }
            }
            else
                target.EnableTriggeredEffect(TriggeredEffect.FailStatusClearEffects);
        }

        public void ChangeTurnTimer(int value)
        {
            m_turnTimer += value;
            if (m_turnTimer < 0)
                m_turnTimer = 0;
        }

        public void ClearSlot()
        {
            m_status = null;
            m_turnTimer = 0;
        }
    }

    [Serializable]
    [Flags]
    public enum TriggeredEffect
    {
        None = 0,
        FailNextSkillEffect = 1,
        FailStatusClearEffects = 2, // Implemented
        DamageOnSkillUse = 4,
        DebuffImmunity = 8, // Implemented
        Lifesteal = 16 // Implemented
    }

    public class BattleMonster
    {
        public BattleMonster() { }
        public BattleMonster(Monster monster, BattleMonsterID id)
        {
            m_monster = monster;
            m_battleID = id;
            m_currentHP = maxHP;
        }
        
        [SerializeField] protected Monster m_monster;
        public Monster monster
        {
            get { return m_monster; }
        }

        [SerializeField] protected BattleMonsterID m_battleID;
        public BattleMonsterID battleID
        {
            get { return m_battleID; }
        }

        // health
        [SerializeField] protected int m_currentHP;
        public int currentHP
        {
            get { return m_currentHP; }
        }
        // accessors for variables for other classes
        public string monsterName
        {
            get { return m_monster.monsterName; }
        }
        public string monsterDataName
        {
            get { return m_monster.monsterData.monsterName; }
        }
        public int maxHP
        {
            get { return m_monster.maxHP; }
        }
        public int attack
        {
            get { return m_monster.attack; }
        }
        public int defence
        {
            get { return m_monster.defence; }
        }
        public int agility
        {
            get { return m_monster.agility; }
        }

        // status slots
        [SerializeField] protected List<StatusSlot> m_statusSlots = new List<StatusSlot>();
        public List<StatusSlot> statusSlots
        {
            get { return m_statusSlots; }
        }
        // skill slots accessor for other classes
        public List<SkillSlot> skillSlots
        {
            get { return m_monster.skillSlots; }
        }

        // bitwise enum for effects with triggers
        [SerializeField] protected TriggeredEffect m_triggeredEffects;
        public TriggeredEffect triggeredEffects
        {
            get { return m_triggeredEffects; }
        }

        // tracks stat modifiers from buffs
        [SerializeField][Min(0)] protected Dictionary<MonsterBaseStats, float> m_statModifiers = new Dictionary<MonsterBaseStats, float>();

        /// <summary>
        /// Resets the BattleMonster to a default state.
        /// </summary>
        public void ResetBattleMonster()
        {
            // clear stat buffs/debuffs
            for (int i = 1; i < m_statModifiers.Count; i++)
                m_statModifiers[(MonsterBaseStats)i] = 1;

            // reset HP
            m_currentHP = m_monster.maxHP;

            // reset all skill cooldowns
            foreach (SkillSlot skillSlot in m_monster.skillSlots)
            {
                skillSlot.ResetTimer();
            }

            // clear statuses
            m_statusSlots.Clear();

            // clear triggered effects
            m_triggeredEffects = 0;
        }

        /// <summary>
        /// Causes the monster to lose HP.
        /// </summary>
        /// <param name="damage">Amount of HP lost.</param>
        public void TakeDamage(int damage)
        {
            // reduce HP
            m_currentHP -= damage;

            if (m_currentHP <= 0)
            {
                // kill TODO
            }
        }

        /// <summary>
        /// Restores the monster's HP.
        /// </summary>
        /// <param name="value">Amount of HP to restore.</param>
        public void RestoreHealth(int value)
        {
            // increase HP, don't allow monster to have HP over the maximum
            m_currentHP += value;

            if (m_currentHP > m_monster.maxHP)
                m_currentHP = m_monster.maxHP;
        }

        /// <summary>
        /// Apply a modifier to one of the monster's stats.
        /// </summary>
        /// <param name="baseStat">The stat to be affected.</param>
        /// <param name="value">Magnitude of the modifer. Negative values represent a debuff.</param>
        public void ApplyStatModifier(MonsterBaseStats baseStat, int value)
        {
            // apply percentage multiplier to specified stat (compounding with other boosts)
            // note: negative values represent a debuff

            // turns value into a 1.X multiplier where X is value
            // or 1 / 1.X value, if debuff
            // if value is negative, make positive by multiplying by -1 and get reciprocal
            float modifier = value < 0 ? -1.0f / (value / 100.0f + 1) : value / 100.0f + 1;
            m_statModifiers[baseStat] *= modifier;
        }

        /// <summary>
        /// Adds a status to the monster's Status Slots.
        /// </summary>
        /// <param name="status">The status to be applied to the monster.</param>
        public void GainStatus(Status status)
        {
            // if target does not have debuff immunity
            if ((m_triggeredEffects & TriggeredEffect.DebuffImmunity) == 0)
            {
                // add new status to status list, then call its on apply affects
                m_statusSlots.Add(new StatusSlot(status));
                m_statusSlots.Last().OnApply(this);
            }
            // if target does have debuff immunity, then don't apply the status
        }

        /// <summary>
        /// Changes the turn timer of a Skill Slot at a specified index.
        /// </summary>
        /// <param name="index">Index of the skill slot.</param>
        /// <param name="value">Amount to change the turn timer.</param>
        public void ChangeSkillCooldown(int index, int value)
        {
            // increase/decrease the turn timer of the skill at the indicated index
            m_monster.skillSlots[index].ChangeTurnTimer(value);
        }

        /// <summary>
        /// Change the turn timer of a Status Slot at a specified index.
        /// </summary>
        /// <param name="index">Index of the status slot.</param>
        /// <param name="value">Amount to change the turn timer.</param>
        public void ChangeStatusTimer(int index, int value)
        {
            // increase/decrease the turn timer of the status at the indicated index
            m_statusSlots[index].ChangeTurnTimer(value);
        }

        /// <summary>
        /// Enables specified effects from TriggeredEffect.
        /// </summary>
        /// <param name="effect">The effect to enable.</param>
        public void EnableTriggeredEffect(TriggeredEffect effect)
        {
            // enable the effect
            m_triggeredEffects |= effect;
        }

        /// <summary>
        /// Disables specified effects from TriggeredEffect.
        /// </summary>
        /// <param name="effect">The effect to disable.</param>
        public void DisableTriggeredEffect(TriggeredEffect effect)
        {
            m_triggeredEffects &= ~effect;
        }

        /// <summary>
        /// Toggles specified effects from TriggeredEffect.
        /// </summary>
        /// <param name="effect">The effect to toggle.</param>
        public void ToggleTriggeredEffect(TriggeredEffect effect)
        {
            m_triggeredEffects ^= effect;
        }

        /// <summary>
        /// Adds a Status Slot to the monster.
        /// Can be created with or without a status.
        /// </summary>
        /// <param name="status">The Status to add.</param>
        public void AddStatusSlot(Status status = null)
        {
            if (status == null)
                // add a new empty status slot
                m_statusSlots.Add(new StatusSlot());
            else
                // add a status slot with the predefined values
                m_statusSlots.Add(new StatusSlot(status));
        }

        /// <summary>
        /// Removes a Status Slot from the monster.
        /// Either removes the newest Status Slot or the Status Slot with a Status matching the param.
        /// </summary>
        /// <param name="status">Will remove the Status Slot with this Status.</param>
        /// <exception cref="IndexOutOfRangeException">If a status slot is attempted to be removed when there aren't any.</exception>
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
                throw new WarningException(monster.monsterName + " has no status slots to remove.");
        }
    }
}

