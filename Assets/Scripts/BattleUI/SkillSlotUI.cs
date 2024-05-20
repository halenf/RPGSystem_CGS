using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RPGSystem
{
    public class SkillSlotUI : ObjectUI
    {
        protected BattleScene m_battleScene;
        protected BattleMonster m_battleMonster;
        protected int m_skillSlotIndex;

        protected Button m_button;
        protected bool isReady
        {
            get { return m_battleMonster.skillSlots[m_skillSlotIndex].turnTimer == 0; }
        }

        [SerializeField] protected TextMeshProUGUI m_textDisplay;
        [SerializeField] protected Image m_spriteDisplay;

        public void Initialise(BattleScene battleScene, BattleMonster battleMonster, int skillIndex)
        {
            m_battleScene = battleScene;
            m_battleMonster = battleMonster;
            m_skillSlotIndex = skillIndex;

            // Initialise button
            m_button = GetComponent<Button>();
            m_button.onClick.AddListener(AddUserSkillToBattleScene);

            UpdateUI();
        }
        
        protected void AddUserSkillToBattleScene()
        {
            m_battleScene.AddUserSkillToAction(m_battleMonster.battleID, m_skillSlotIndex);
        }

        public override void UpdateUI()
        {
            // display skill
            m_textDisplay.text = m_battleMonster.skillSlots[m_skillSlotIndex].skill.name;
            m_spriteDisplay.sprite = m_battleMonster.skillSlots[m_skillSlotIndex].skill.sprite;
        }
    }
}
