using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    public class HealthBarUI : BarUI
    {
        [HideInInspector] public BattleUnit unit;
        
        public override void UpdateUI()
        {
            int maxHP = unit.maxHP;
            int currentHP = unit.currentHP;

            m_slider.maxValue = maxHP;
            m_slider.value = currentHP;

            if (m_showTextValue)
                m_textValueDisplay.text = currentHP.ToString() + "/" + maxHP.ToString();
        }
    }
}

