using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    [Serializable]
    public struct CharacterSkill
    {
        private Skill m_skill;
        private int m_cost;
    }

    [CreateAssetMenu(fileName = "Character", menuName = "RPGSystem_SO/Character", order = 1)]
    public class Character : ScriptableObject
    {
        // personal character details
        [SerializeField] private string m_characterName;
        [SerializeField] private Sprite m_icon;
        
        // array of characters monsters
        [SerializeField] private Monster[] m_monsters = new Monster[3];

        // character's available character skills
        [SerializeField] private CharacterSkill[] m_characterSkills = new CharacterSkill[3];

        // base stats
        private int m_skill;

        // public accessors
        public string characterName
        {
            get { return m_characterName; }
        }
        public Sprite icon
        {
            get { return m_icon; }
        }
        public Monster[] monsters
        {
            get { return m_monsters; }
        }
        public CharacterSkill[] characterSkills
        {
            get { return m_characterSkills; }
        }
        public int skill
        {
            get { return m_skill; }
        }

        // volatile stats
        private int m_currentAP;
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
    }
}
