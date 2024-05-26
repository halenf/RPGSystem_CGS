using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    [Serializable]
    public class CharacterSkillSlot
    {
        [SerializeField] private Skill m_skill;
        [SerializeField] private int m_cost;

        public Skill skill
        { get { return m_skill; } }
        public int cost
        { get { return m_cost; } }

        public void ClearSlot()
        {
            m_skill = null;
            m_cost = 0;
        }
    }

    [CreateAssetMenu(fileName = "Character", menuName = "RPGSystem/Character", order = 1)]
    public class Character : ScriptableObject
    {
        // personal character details
        [SerializeField] protected string m_characterName;
        [SerializeField] protected Sprite m_sprite;
        
        // array of characters units
        [SerializeField] protected Unit[] m_units = new Unit[3];

        // character's available character skills
        [SerializeField] protected CharacterSkillSlot[] m_characterSkillSlots = new CharacterSkillSlot[GameSettings.MAX_SKILLS_PER_CHARACTER];

        // base stats
        protected int m_skill;

        // public accessors
        public string characterName
        {
            get { return m_characterName; }
        }
        public Sprite sprite
        {
            get { return m_sprite; }
        }
        public Unit[] units
        {
            get { return m_units; }
        }
        public CharacterSkillSlot[] characterSkillSlots
        {
            get { return m_characterSkillSlots; }
        }
        public int skill
        {
            get { return m_skill; }
        }

        // volatile stats
        protected int m_currentAP;
        public int maxAP
        {
            get
            {
                return (int)(20.0f / (float)Mathf.Log(m_skill) + 10.0f);
            }
        }
        public int currentAP
        {
            get { return m_currentAP; }
        }

        public void ChangeAP(int value)
        {
            m_currentAP += value;

            // keep AP under max and above 0 (inclusive)
            if (m_currentAP > maxAP)
                m_currentAP = maxAP;
            if (m_currentAP < 0)
                m_currentAP = 0;
        }

        public void ResetBattleCharacter()
        {
            m_currentAP = maxAP;
        }

        public void ResetCharacter()
        {
            ResetBattleCharacter();
            m_units = null;
            m_characterName = string.Empty;
            m_sprite = null;
            m_skill = 1;
        }
    }
}
