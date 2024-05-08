using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGSystem
{
    public class CharacterUI : ObjectUI
    {       
        [HideInInspector] public Character character;
        private MonsterUI[] m_monsterUIArray = new MonsterUI[3];
        private APBarUI m_currentAPBar;
        
        // object references
        [SerializeField] private TextMeshProUGUI m_characterNameDisplay;
        [SerializeField] private Image m_characterIconDisplay;

        public void Initialise()
        {
            // let the slider access the character details
            m_currentAPBar = GetComponentInChildren<APBarUI>();
            m_currentAPBar.character = character;

            // set icon if available
            if (character.icon != null)
                m_characterIconDisplay.sprite = character.icon;

            // Get the monsterUIs attached to this object
            m_monsterUIArray = GetComponentsInChildren<MonsterUI>();

            // initialise the monsterUIs
            for (int i = 0; i < m_monsterUIArray.Length; i++)
            {
                m_monsterUIArray[i].monster = character.monsters[i];
                m_monsterUIArray[i].Initialise();
            }
        }

        public override void UpdateUI()
        {
            // monster UI
            foreach (MonsterUI ui in m_monsterUIArray)
            {
                ui.UpdateUI();
            }
            
            // name display
            m_characterNameDisplay.text = character.characterName;

            // ap slider display
            m_currentAPBar.UpdateUI();
        }
    }
}
