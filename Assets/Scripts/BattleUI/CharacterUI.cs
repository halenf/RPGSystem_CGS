using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGSystem
{
    public class CharacterUI : ObjectUI
    {       
        [SerializeField] private Character m_character;
        private BattleMonsterUI[] m_battleMonsterUIArray;
        private APBarUI m_currentAPBar;

        // object references
        public BattleMonsterUI battleMonsterUIPrefab;
        public Transform battleMonsterUIDisplayArea;
        [SerializeField] private TextMeshProUGUI m_characterNameDisplay;
        [SerializeField] private Image m_characterIconDisplay;

        public void Initialise(Character character)
        {
            // Set character
            m_character = character;
            
            // let the slider access the character details
            m_currentAPBar = GetComponentInChildren<APBarUI>();
            m_currentAPBar.character = character;

            // name display
            m_characterNameDisplay.text = m_character.characterName;

            // set icon if available
            if (character.icon != null)
                m_characterIconDisplay.sprite = character.icon;

            // Instantiate the BattleMonsterUIs
            m_battleMonsterUIArray = new BattleMonsterUI[character.monsters.Length];
            for (int i = 0; i < m_battleMonsterUIArray.Length; i++)
            {
                m_battleMonsterUIArray[i] = Instantiate(battleMonsterUIPrefab, battleMonsterUIDisplayArea);
                m_battleMonsterUIArray[i].Initialise(new BattleMonster(character.monsters[i]));
            }
        }

        public override void UpdateUI()
        {
            // battlemonster UI update
            foreach (BattleMonsterUI ui in m_battleMonsterUIArray)
            {
                ui.UpdateUI();
            }

            // ap slider display update
            m_currentAPBar.UpdateUI();
        }
    }
}
