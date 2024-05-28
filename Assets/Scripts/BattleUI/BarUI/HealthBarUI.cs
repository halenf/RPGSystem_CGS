using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    public class HealthBarUI : BarUI
    {
        private BattleUnit m_unit;

        public void Initialise(BattleUnit battleUnit)
        {
            m_unit = battleUnit;
        }
        
        public override void UpdateUI()
        {
            int maxHP = m_unit.maxHP;
            int currentHP = m_unit.currentHP;

            m_slider.maxValue = maxHP;
            m_slider.value = currentHP;

            if (m_showTextValue)
                m_textValueDisplay.text = currentHP.ToString() + "/" + maxHP.ToString();
        }
    }
}

