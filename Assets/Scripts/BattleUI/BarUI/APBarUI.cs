using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    public class APBarUI : BarUI
    {
        [HideInInspector] public Character character;

        public override void UpdateUI()
        {
            int maxAP = character.maxAP;
            int currentAP = character.currentAP;

            m_slider.maxValue = maxAP;
            m_slider.value = currentAP;

            if (m_showTextValue)
                m_textValueDisplay.text = currentAP.ToString() + "/" + maxAP.ToString();
        }
    }
}
