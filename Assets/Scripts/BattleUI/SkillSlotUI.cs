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
        protected BattleUnit m_battleUnit;
        protected int m_skillSlotIndex;

        protected Button m_button;
        public bool isReady
        {
            get { return m_battleUnit.skillSlots[m_skillSlotIndex].turnTimer == 0; }
        }

        [SerializeField] protected TextMeshProUGUI m_textDisplay;
        protected Image m_spriteDisplay;

        public void Initialise(BattleScene battleScene, BattleUnit battleUnit, int skillIndex)
        {
            m_battleScene = battleScene;
            m_battleUnit = battleUnit;
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
            if (m_textDisplay)
                m_textDisplay.text = m_battleUnit.skillSlots[m_skillSlotIndex].skill.name;
            if (m_spriteDisplay)
                m_spriteDisplay.sprite = m_battleUnit.skillSlots[m_skillSlotIndex].skill.sprite;
        }
    }
}
