using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{   
    [CreateAssetMenu(fileName = "Skill", menuName = "RPGSystem_SO/Skill", order = 1)]
    public class Skill : ScriptableObject
    {
        [SerializeField] private string m_skillName;
        [SerializeField] private int m_turnTimer;

        [SerializeField] private SkillStatusEffect[] m_beforeHitEffects;
        [SerializeField] private SkillStatusEffect[] m_onHitEffects;
        [SerializeField] private SkillStatusEffect[] m_afterHitEffects;

        public string skillName
        {
            get
            {
                return m_skillName;
            }
        }
        public int turnTimer
        {
            get
            {
                return m_turnTimer;
            }
        }

        public SkillStatusEffect[] beforeHitEffects
        {
            get
            {
                return m_beforeHitEffects;
            }
        }
        public SkillStatusEffect[] onHitEffects
        {
            get
            {
                return m_onHitEffects;
            }
        }
        public SkillStatusEffect[] afterHitEffects
        {
            get
            {
                return m_afterHitEffects;
            }
        }
    }
}
