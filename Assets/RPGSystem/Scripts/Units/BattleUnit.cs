using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

namespace RPGSystem
{
    [Serializable]
    public abstract class BattleUnit
    {
        public BattleUnit() { }
        public BattleUnit(Unit unit, BattleUnitID id)
        {
            m_unit = unit;
            m_battleID = id;
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

        public string displayName { get { return m_unit.displayName; } }

        // health
        public int currentHP
        {
            get { return m_unit.currentHP; }
            set { m_unit.currentHP = value; }
        }

        // slot slots
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
        [SerializeField][Min(0)] protected Dictionary<BaseStatName, float> m_statModifiers = new Dictionary<BaseStatName, float>();

        public int GetStat(BaseStatName stat)
        {
            return m_unit.GetStat(stat);
        }

        private void InitialiseStatModifiers()
        {
            m_statModifiers = new Dictionary<BaseStatName, float>();
            for (int i = 0; i < GameSettings.STAT_NAMES.Length; i++)
            {
                m_statModifiers.Add((BaseStatName)i, 1.0f);
            }
        }

        /// <summary>
        /// Resets the BattleUnit to a default state.
        /// </summary>
        public virtual void ResetBattleUnit()
        {
            // clear stat buffs/debuffs
            InitialiseStatModifiers();

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
        public bool TakeDamage(int damage)
        {
            if (m_triggeredEffects.HasFlag(TriggeredEffect.Invulnerable))
            {
                BattleTextLog.Instance.AddLine(displayName + " is Invulnerable and blocks the instance of damage!");
                return false;
            }

            // reduce HP
            m_unit.currentHP -= damage;

            if (m_unit.currentHP <= 0)
            {
                m_unit.currentHP = 0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Restores the unit's HP.
        /// </summary>
        /// <param name="value">Amount of HP to restore.</param>
        public void RestoreHealth(int value)
        {
            // increase HP
            currentHP += value;
        }

        /// <summary>
        /// Apply a modifier to one of the unit's stats.
        /// </summary>
        /// <param name="baseStat">The stat to be affected.</param>
        /// <param name="value">Magnitude of the modifer. Negative values represent a debuff.</param>
        public void ApplyStatModifier(BaseStatName baseStat, int value)
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
        /// Adds a slot to the unit's Status Slots.
        /// </summary>
        /// <param name="status">The slot to be applied to the unit.</param>
        public void GainStatus(Status status)
        {
            // if targets does not have debuff immunity
            if (status.statusType == Status.StatusType.Debuff && m_triggeredEffects.HasFlag(TriggeredEffect.DebuffImmunity))
                BattleTextLog.Instance.AddLine("But " + displayName + " is immune to debuffs!");
            // if unit already has this slot, extend it
            else if (m_statusSlots.Find(slot => slot.status.statusName == status.statusName) != null)
            {
                StatusSlot slot = m_statusSlots.Find(slot => slot.status.statusName == status.statusName);
                slot.ChangeTurnTimer(status.turnTimer);
                BattleTextLog.Instance.AddLine(displayName + "'s " + status.statusName + " was extended by " + status.turnTimer.ToString() + " turns!");
            }
            else
            {
                // add new slot to slot list, then call its on apply affects
                m_statusSlots.Add(new StatusSlot(status));
                m_statusSlots.Last().OnApply(this);
            }
            // if targets does have debuff immunity, then don't apply the slot
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
        /// <param name="index">Index of the slot slot.</param>
        /// <param name="value">Amount to change the turn timer.</param>
        public void ChangeStatusTimer(int index, int value)
        {
            // increase/decrease the turn timer of the slot at the indicated index
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

        public void CheckStatusTimers()
        {
            List<StatusSlot> statusesToRemove = new List<StatusSlot>();
            foreach (StatusSlot slot in m_statusSlots)
            {
                // slot on clear effects
                if (slot.turnTimer == 0)
                    statusesToRemove.Add(slot);
            }

            // clear any slots that need to be cleared
            foreach (StatusSlot slot in statusesToRemove)
            {
                BattleTextLog.Instance.AddLine(displayName + "'s " + slot.status.statusName + " is cleared!");
                slot.OnClear(this);
                RemoveStatusSlot(slot.status);
            }
        }

        /// <summary>
        /// Adds a Status Slot to the unit.
        /// Can be created with or without a slot.
        /// </summary>
        /// <param name="status">The Status to add.</param>
        public void AddStatusSlot(Status status = null)
        {
            if (status == null)
                // add a new empty slot slot
                m_statusSlots.Add(new StatusSlot());
            else
                // add a slot slot with the predefined values
                m_statusSlots.Add(new StatusSlot(status));
        }

        /// <summary>
        /// Removes a Status Slot from the unit.
        /// Either removes the newest Status Slot or the Status Slot with a Status matching the param.
        /// </summary>
        /// <param name="status">Will remove the Status Slot with this Status.</param>
        public void RemoveStatusSlot(Status status = null)
        {
            if (m_statusSlots.Count > 0)
            {
                if (status == null)
                    // remove the last slot slot in the list
                    m_statusSlots.RemoveAt(m_statusSlots.Count - 1);
                else
                    // removes all slot slots that have a matching slot
                    // a unit shouldn't be able to have two of the same slot so it should only remove one slot slot,
                    // if it has that slot
                    if (m_statusSlots.RemoveAll(statusSlot => statusSlot.status.statusName == status.statusName) == 0)
                        Debug.LogError(displayName + " does not have a StatusSlot with " + status.statusName + ".");
            }
            else
                Debug.LogError(displayName + " has no StatusSlots to remove.");
        }
    }
}

