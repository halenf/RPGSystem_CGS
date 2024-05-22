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
        public bool isReady
        {
            get { return m_battleMonster.skillSlots[m_skillSlotIndex].turnTimer == 0; }
        }

        [SerializeField] protected TextMeshProUGUI m_textDisplay;
        protected Image m_spriteDisplay;

        public void Initialise(BattleScene battleScene, BattleMonster battleMonster, int skillIndex)
        {
            m_battleScene = battleScene;
            m_battleMonster = battleMonster;
            m_skillSlotIndex = skillIndex;

            // Get image component
            m_spriteDisplay = GetComponent<Image>();

            // Initialise button
            m_button = GetComponent<Button>();
            m_button.onClick.AddListener(AddSkillToBattleScene);
            SetAsAvailableSkill(false);

            UpdateUI();
        }
        
        protected void AddSkillToBattleScene()
        {
            m_battleScene.AddSkillToAction(m_skillSlotIndex);
        }

        public void SetAsAvailableSkill(bool value)
        {
            if (value && isReady)
                m_button.interactable = true;
            else
                m_button.interactable = false;
        }

        public override void UpdateUI()
        {
            // display skill
            m_textDisplay.text = m_battleMonster.skillSlots[m_skillSlotIndex].skill.name;
            m_spriteDisplay.sprite = m_battleMonster.skillSlots[m_skillSlotIndex].skill.sprite;
        }
    }
}
