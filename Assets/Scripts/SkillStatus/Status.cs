using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    [System.Serializable]
    public enum StatusType
    {
        Buff,
        Debuff
    }

    [CreateAssetMenu(fileName = "Status", menuName = "RPGSystem/Status", order = 1)]
    public class Status : ScriptableObject
    {
        /// <summary>
        /// The unit that applied the status.
        /// </summary>
        protected BattleUnit m_user;
        /// <summary>
        /// Whether this status is a buff or debuff.
        /// </summary>
        [SerializeField] protected StatusType m_statusType;
        /// <summary>
        /// The default number of turns the status is active for.
        /// </summary>
        [SerializeField] protected int m_turnTimer;

        // effect arrays for when the status is applied, cleared, and effects that occur at the start and end of every battle turn
        [SerializeField] protected SkillStatusEffect[] m_onApply;
        [SerializeField] protected SkillStatusEffect[] m_onTurnStart;
        [SerializeField] protected SkillStatusEffect[] m_onTurnEnd;
        [SerializeField] protected SkillStatusEffect[] m_onClear;

        public BattleUnit user
        {
            get
            {
                return m_user;
            }
        }
        public StatusType statusType
        {
            get
            {
                return m_statusType;
            }
        }
        public int turnTimer
        {
            get
            {
                return m_turnTimer;
            }
        }

        public SkillStatusEffect[] onApply
        {
            get
            {
                return m_onApply;
            }
        }

        public SkillStatusEffect[] onTurnStart
        {
            get { return m_onTurnStart; }
        }

        public SkillStatusEffect[] onTurnEnd
        {
            get
            {
                return m_onTurnEnd;
            }
        }
        public SkillStatusEffect[] onClear
        {
            get
            {
                return m_onClear;
            }
        }
    }
}
