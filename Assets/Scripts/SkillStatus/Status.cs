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
        // the monster that applied the status
        protected BattleMonster m_user;
        // whether this status is a buff or debuff
        [SerializeField] protected StatusType m_statusType;
        // the default number of turns the status is active for
        [SerializeField] protected int m_turnTimer;

        // effect arrays for when the status is applied, cleared, and effects that occur at the end of every battle turn
        [SerializeField] protected SkillStatusEffect[] m_onApply;
        [SerializeField] protected SkillStatusEffect[] m_onTurnEnd;
        [SerializeField] protected SkillStatusEffect[] m_onClear;

        public BattleMonster user
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
