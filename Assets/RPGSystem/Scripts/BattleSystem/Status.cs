using System;
using UnityEngine;

namespace RPGSystem
{
    [CreateAssetMenu(fileName = "Status", menuName = "RPGSystem/Status", order = 1)]
    public class Status : ScriptableObject
    {       
        /// <summary>
        /// Name of the Status.
        /// </summary>
        [SerializeField] protected string m_statusName;
        /// <summary>
        /// The default number of turns the status is active for.
        /// </summary>
        [SerializeField] protected int m_turnTimer;
        /// <summary>
        /// The unit that applied the status.
        /// </summary>
        protected BattleUnit m_user;
        /// <summary>
        /// The type of the Status.
        /// </summary>
        [SerializeField] protected StatusType m_statusType;

        // effect arrays for when the status is applied, cleared, and effects that occur at the start and end of every battle turn
        [SerializeField] protected Effect[] m_onApply;
        [SerializeField] protected Effect[] m_onTurnStart;
        [SerializeField] protected Effect[] m_onTurnEnd;
        [SerializeField] protected Effect[] m_onClear;

        [System.Serializable]
        public enum StatusType
        {
            Default,
            Buff,
            Debuff
        }

        public string statusName { get { return m_statusName;} }
        public BattleUnit user { get { return m_user; } }
        public StatusType statusType {  get { return m_statusType; } }
        public int turnTimer { get { return m_turnTimer; } }

        public Effect[] onApply { get { return m_onApply; } }
        public Effect[] onTurnStart { get { return m_onTurnStart; } }
        public Effect[] onTurnEnd { get { return m_onTurnEnd; } }
        public Effect[] onClear { get { return m_onClear; } }

        public void SetUser(BattleUnit unit)
        {
            m_user = unit;
        }
    }

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
            foreach (Effect effect in m_status.onApply)
            {
                effect.DoEffect(m_status.user, new BattleUnit[] { target });
            }
        }

        /// <summary>
        /// Activates the end of turn effects of the status.
        /// </summary>
        /// <param name="target"></param>
        public void OnTurnEnd(BattleUnit target)
        {
            foreach (Effect effect in m_status.onTurnEnd)
            {
                effect.DoEffect(m_status.user, new BattleUnit[] { target });
            }
        }

        /// <summary>
        /// Activates the on clear effects of the status.
        /// </summary>
        /// <param name="target"></param>
        public void OnClear(BattleUnit target)
        {
            // if clear effects on this targets should not fail
            if ((target.triggeredEffects & TriggeredEffect.FailStatusClearEffects) == 0)
            {
                foreach (Effect effect in m_status.onClear)
                {
                    effect.DoEffect(m_status.user, new BattleUnit[] { target });
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
}
