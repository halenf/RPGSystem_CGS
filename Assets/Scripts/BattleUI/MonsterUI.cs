using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGSystem
{
    public class MonsterUI : ObjectUI
    {       
        [HideInInspector] public Monster monster;
        private HealthBarUI m_currentHealthBar;
        private bool m_facingRight;

        [SerializeField] private TextMeshProUGUI m_monsterNameDisplay;
        [SerializeField] private Image m_monsterSpriteDisplay;

        public void Initialise()
        {
            m_currentHealthBar = GetComponentInChildren<HealthBarUI>();
            m_currentHealthBar.monster = monster;
        }

        public override void UpdateUI()
        {
            // name display set to monster name
            m_monsterNameDisplay.text = monster.monsterData.name;

            // if monster has sprite, display it
            if (monster.monsterData.monsterSprite != null)
                m_monsterSpriteDisplay.sprite = monster.monsterData.monsterSprite;
            // if the monster should face the other way, flip it by inverting the scale
            if (m_facingRight)
            {
                Vector3 scale = m_monsterSpriteDisplay.rectTransform.localScale;
                m_monsterSpriteDisplay.rectTransform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            }

            // update health bar display
            m_currentHealthBar.UpdateUI();
        }
    }
}

