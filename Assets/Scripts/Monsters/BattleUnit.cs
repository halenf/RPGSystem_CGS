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
        public void OnApply(BattleUnit target)
        {
            foreach (SkillStatusEffect effect in m_status.onApply)
            {
                effect.Effect(m_status.user, new BattleUnit[] { target });
            }
        }

        /// <summary>
        /// Activates the end of turn effects of the status.
        /// </summary>
        /// <param name="target"></param>
        public void OnTurnEnd(BattleUnit target)
        {
            foreach (SkillStatusEffect effect in m_status.onTurnEnd)
            {
                effect.Effect(m_status.user, new BattleUnit[] { target });
            }
        }

        /// <summary>
        /// Activates the on clear effects of the status.
        /// </summary>
        /// <param name="target"></param>
        public void OnClear(BattleUnit target)
        {
            // if clear effects on this target should not fail
            if ((target.triggeredEffects & TriggeredEffect.FailStatusClearEffects) == 0)
            {
                foreach (SkillStatusEffect effect in m_status.onClear)
                {
                    effect.Effect(m_status.user, new BattleUnit[] { target });
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

    public class BattleUnit
    {
        public BattleUnit() { }
        public BattleUnit(Unit unit, BattleUnitID id)
        {
            m_unit = unit;
            m_battleID = id;
            currentHP = maxHP;
        }
        
        [SerializeField] protected Unit m_unit;
        public Unit unit
        {
            get { return m_unit; }
        }

        [SerializeField] protected BattleUnitID m_battleID;
        public BattleUnitID battleID
        {
            get { return m_battleID; }
        }

        // accessors for variables for other classes
        public string unitNickname
        {
            get { return m_unit.unitNickname; }
        }
        public string unitName
        {
            get { return m_unit.unitData.unitName; }
        }
        // health
        public int currentHP
        {
            get { return m_unit.currentHP; }
            set { m_unit.currentHP = value; }
        }
        public int maxHP
        {
            get { return m_unit.maxHP; }
        }
        public int attack
        {
            get { return m_unit.attack; }
        }
        public int defence
        {
            get { return m_unit.defence; }
        }
        public int speed
        {
            get { return m_unit.speed; }
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
            get { return m_unit.skillSlots; }
        }

        // bitwise enum for effects with triggers
        [SerializeField] protected TriggeredEffect m_triggeredEffects;
        public TriggeredEffect triggeredEffects
        {
            get { return m_triggeredEffects; }
        }

        // tracks stat modifiers from buffs
        [SerializeField][Min(0)] protected Dictionary<UnitBaseStatNames, float> m_statModifiers = new Dictionary<UnitBaseStatNames, float>();

        /// <summary>
        /// Resets the BattleUnit to a default state.
        /// </summary>
        public void ResetBattleUnit()
        {
            // clear stat buffs/debuffs
            for (int i = 1; i < m_statModifiers.Count; i++)
                m_statModifiers[(UnitBaseStatNames)i] = 1;

            // reset HP
            currentHP = maxHP;

            // reset all skill cooldowns
            foreach (SkillSlot skillSlot in m_unit.skillSlots)
            {
                skillSlot.ResetTimer();
            }

            // clear statuses
            m_statusSlots.Clear();

            // clear triggered effects
            m_triggeredEffects = 0;
        }

        /// <summary>
        /// Causes the unit to lose HP.
        /// </summary>
        /// <param name="damage">Amount of HP lost.</param>
        public void TakeDamage(int damage)
        {
            // reduce HP
            currentHP -= damage;

            if (currentHP <= 0)
            {
                // kill TODO
            }
        }

        /// <summary>
        /// Restores the unit's HP.
        /// </summary>
        /// <param name="value">Amount of HP to restore.</param>
        public void RestoreHealth(int value)
        {
            // increase HP, don't allow unit to have HP over the maximum
            currentHP += value;

            if (currentHP > m_unit.maxHP)
                currentHP = m_unit.maxHP;
        }

        /// <summary>
        /// Apply a modifier to one of the unit's stats.
        /// </summary>
        /// <param name="baseStat">The stat to be affected.</param>
        /// <param name="value">Magnitude of the modifer. Negative values represent a debuff.</param>
        public void ApplyStatModifier(UnitBaseStatNames baseStat, int value)
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
        /// Adds a status to the unit's Status Slots.
        /// </summary>
        /// <param name="status">The status to be applied to the unit.</param>
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
            m_unit.skillSlots[index].ChangeTurnTimer(value);
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
        /// Adds a Status Slot to the unit.
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
        /// Removes a Status Slot from the unit.
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
                    // a unit shouldn't be able to have two of the same status so it should only remove one status slot,
                    // if it has that status
                    m_statusSlots.RemoveAll(statusSlot => statusSlot.status == status);
            }
            else
                throw new WarningException(unit.unitNickname + " has no status slots to remove.");
        }
    }
}

