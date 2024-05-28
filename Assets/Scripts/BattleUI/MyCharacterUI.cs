using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGSystem
{
    public class MyCharacterUI : ObjectUI
    {       
        private Character m_character;
        [SerializeField] private bool m_isEnemyCharacter;

        // object references
        public Transform battleUnitUIContainer;
        [SerializeField] private APBarUI m_currentAPBar;
        [SerializeField] private TextMeshProUGUI m_characterNameDisplay;
        [SerializeField] private Image m_characterIconDisplay;

        public void Initialise(Character character, int characterIndex)
        {
            // Set character
            m_character = character;

            // let the slider access the character details
            m_currentAPBar.Initialise(m_character);

            UpdateUI();
        }

        public override void UpdateUI()
        {
            // name display
            m_characterNameDisplay.text = m_character.characterName;
            
            // ap slider display update
            m_currentAPBar.UpdateUI();

            // set icon if available
            if (m_character.sprite != null)
                m_characterIconDisplay.sprite = m_character.sprite;
        }
    }
}
