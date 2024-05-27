using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
            m_button.onClick.AddListener(SendSkillAsSkillSlotIndex);
            SetAsAvailableSkill(false);

            UpdateUI();
        }
        
        protected void SendSkillAsSkillSlotIndex()
        {
            m_battleScene.AddSkillToAction(m_skillSlotIndex);
            Debug.Log(m_battleUnit.skillSlots[m_skillSlotIndex].skill.skillName + " selected as skill.");
        }

        public void SetAsAvailableSkill(bool value)
        {
            if (value && isReady)
                m_button.interactable = true;
            else
                m_button.interactable = false;
        }

        public void SetSkillSlotIndex(int value)
        {
            if (value >= GameSettings.MAX_SKILLS_PER_UNIT || value < 0)
                throw new IndexOutOfRangeException("SkillSlot index is out of range.");
            else
                m_skillSlotIndex = value;
        }

        public override void UpdateUI()
        {
            // display skill
            if (m_textDisplay)
                m_textDisplay.text = m_battleUnit.skillSlots[m_skillSlotIndex].skill.name;
        }
    }
}
