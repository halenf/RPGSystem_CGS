namespace RPGSystem
{
    public class APBarUI : BarUI
    {
        private MyCharacter m_character;

        public void Initialise(MyCharacter character)
        {
            m_character = character;
        }

        public override void UpdateUI()
        {
            int maxAP = m_character.maxAP;
            int currentAP = m_character.currentAP;

            m_slider.maxValue = maxAP;
            m_slider.value = currentAP;

            if (m_showTextValue)
                m_textValueDisplay.text = currentAP.ToString() + "/" + maxAP.ToString();
        }
    }
}
