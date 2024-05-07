using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    public class HealthBarUI : BarUI
    {
        [HideInInspector] public Monster monster;
        
        public override void UpdateUI()
        {
            int maxHP = monster.maxHP;
            int currentHP = monster.currentHP;

            m_slider.maxValue = maxHP;
            m_slider.value = currentHP;

            m_textValueDisplay.text = currentHP.ToString() + "/" + maxHP.ToString();
        }
    }
}

