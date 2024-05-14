using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGSystem
{
    public class BattleMonsterUI : ObjectUI
    {       
        [SerializeField] private BattleMonster m_battleMonster;
        private HealthBarUI m_currentHealthBar;
        private bool m_facingRight;

        [SerializeField] private TextMeshProUGUI m_monsterNameDisplay;
        [SerializeField] private Image m_monsterSpriteDisplay;

        public void Initialise(BattleMonster battleMonster)
        {
            // set battle monster
            m_battleMonster = battleMonster;

            // Init health bar
            m_currentHealthBar = GetComponentInChildren<HealthBarUI>();
            m_currentHealthBar.monster = battleMonster;

            // name display set to monster name
            m_monsterNameDisplay.text = m_battleMonster.monster.monsterData.name;

            // if monster has sprite, display it
            Sprite monsterSprite = m_battleMonster.monster.monsterData.monsterSprite;
            if (monsterSprite != null)
                m_monsterSpriteDisplay.sprite = monsterSprite;
            // if the monster should face the other way, flip it by inverting the scale
            if (m_facingRight)
            {
                Vector3 scale = m_monsterSpriteDisplay.rectTransform.localScale;
                m_monsterSpriteDisplay.rectTransform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            }
        }

        public override void UpdateUI()
        {           
            // update health bar display
            m_currentHealthBar.UpdateUI();
        }
    }
}

