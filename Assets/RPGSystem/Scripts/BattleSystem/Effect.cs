using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    [System.Serializable]
    public abstract class Effect : ScriptableObject
    {
        protected enum EffectPower
        {
            Light = 3,
            Medium = 5,
            Heavy = 8,
            Severe = 11
        }

        protected enum ValueType
        {
            Value,
            UserStat,
            TargetStat,
            Percentage
        }

        // Integer data for the skill effect
        [SerializeField] protected int m_value;
        public int value { get { return m_value; } }

        public abstract void DoEffect(BattleUnit user, BattleUnit[] targets);
    }

}
