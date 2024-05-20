using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{   
    [CreateAssetMenu(fileName = "Skill", menuName = "RPGSystem/Skill", order = 1)]
    public class Skill : ScriptableObject
    {
        [SerializeField] protected string m_skillName;
        [SerializeField] protected Sprite m_sprite;

        [SerializeField] protected int m_turnTimer;

        /// <summary>
        /// Effects trigger in the order set here.
        /// </summary>
        [SerializeField] protected SkillStatusEffect[] m_onHitEffects;

        public string skillName
        {
            get
            {
                return m_skillName;
            }
        }
        public Sprite sprite
        {
            get { return m_sprite; }
        }
        public int turnTimer
        {
            get
            {
                return m_turnTimer;
            }
        }
        public SkillStatusEffect[] onHitEffects
        {
            get
            {
                return m_onHitEffects;
            }
        }
    }
}
