using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace RPGSystem
{
    public class BattleMonsterUI : ObjectUI
    {
        protected BattleScene m_battleScene;
        protected BattleMonster m_battleMonster;
        protected HealthBarUI m_currentHealthBar;
        protected bool m_facingLeft;

        protected Button m_button;
        [SerializeField] protected TextMeshProUGUI m_monsterNameDisplay;
        [SerializeField] protected Image m_monsterSpriteDisplay;

        public void Initialise(BattleScene battleScene, BattleMonster battleMonster, bool isEnemyMonster)
        {
            // set BattleMonster
            m_battleMonster = battleMonster;
            m_facingLeft = isEnemyMonster;

            // init button
            m_button = GetComponent<Button>();
            m_button.onClick.AddListener(SendMonsterAsTarget);

            // Init health bar
            m_currentHealthBar = GetComponentInChildren<HealthBarUI>();
            if (m_currentHealthBar)
                m_currentHealthBar.monster = m_battleMonster;
        }

        public override void UpdateUI()
        {           
            // update health bar display
            if (m_currentHealthBar)
                m_currentHealthBar.UpdateUI();

            // name display set to monster name
            string nameText = m_battleMonster.monsterName;
            if (m_battleMonster.monsterName != m_battleMonster.monsterDataName)
                nameText += " (" + m_battleMonster.monsterDataName + ")";
            m_monsterNameDisplay.text = nameText;

            // if monster has sprite, display it
            Sprite monsterSprite = m_battleMonster.monster.monsterData.monsterSprite;
            if (monsterSprite != null)
                m_monsterSpriteDisplay.sprite = monsterSprite;

            // if the monster should face the other way, flip it by inverting the scale
            if (m_facingLeft)
            {
                Vector3 scale = m_monsterSpriteDisplay.rectTransform.localScale;
                m_monsterSpriteDisplay.rectTransform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            }
        }

        // need to send one, have buffer in battle scene
        protected void SendMonsterAsTarget()
        {
            //m_battleScene.AddTargetsToAction(m_battleMonster.battleID);
        }
    }
}

