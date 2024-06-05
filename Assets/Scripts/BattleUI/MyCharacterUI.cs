using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGSystem
{
    public class MyCharacterUI : ObjectUI
    {       
        private MyCharacter m_character;
        [SerializeField] private bool m_isEnemyCharacter;

        // object references
        public Transform battleUnitUIContainer;
        [SerializeField] private TextMeshProUGUI m_characterNameDisplay;
        [SerializeField] private Image m_characterIconDisplay;

        public void Initialise(MyCharacter character, int characterIndex)
        {
            // Set character
            m_character = character;

            UpdateUI();
        }

        public override void UpdateUI()
        {
            // name display
            m_characterNameDisplay.text = m_character.characterName;

            // set icon if available
            if (m_character.sprite != null)
                m_characterIconDisplay.sprite = m_character.sprite;
        }
    }
}
