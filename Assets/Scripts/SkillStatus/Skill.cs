using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    [Serializable]
    public enum TargetType
    {
        SingleEnemy = 0,
        Self = 1,
        AdjacentEnemies = 2,
        AllEnemies = 4,
        SingleParty = 8,
        WholePartyButSelf = 16,
        WholeParty = 32,
        EveryoneButSelf = 64,
        Everyone = 128
    }

    [CreateAssetMenu(fileName = "Skill", menuName = "RPGSystem/Skill", order = 1)]
    public class Skill : ScriptableObject
    {
        [SerializeField] protected string m_skillName;
        [SerializeField] protected Sprite m_sprite;

        [SerializeField] protected int m_turnTimer;

        [SerializeField] protected TargetType m_target;

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
        public TargetType target
        {
            get { return m_target; }
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