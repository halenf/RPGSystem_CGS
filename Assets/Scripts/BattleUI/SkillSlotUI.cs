using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace RPGSystem
{
    public class SkillSlotUI : ObjectUI
    {
        private MyBattleScene m_battleScene;
        private BattleUnit m_battleUnit;
        private int m_skillSlotIndex;

        private Button m_button;
        public bool isReady
        {
            get { return m_battleUnit.skillSlots[m_skillSlotIndex].turnTimer == 0; }
        }

        [SerializeField] private TextMeshProUGUI m_textDisplay;
        [SerializeField] private TextMeshProUGUI m_turnTimerDisplay;

        public void Initialise(MyBattleScene battleScene, BattleUnit battleUnit, int skillIndex)
        {
            m_battleScene = battleScene;
            m_battleUnit = battleUnit;
            m_skillSlotIndex = skillIndex;

            // Initialise button
            m_button = GetComponent<Button>();
            m_button.onClick.AddListener(SendSkillAsSkillSlotIndex);
            SetAsAvailableSkill(false);

            UpdateUI();
        }
        
        private void SendSkillAsSkillSlotIndex()
        {
            m_battleScene.AddSkillToCurrentAttackAction(m_skillSlotIndex);
            transform.parent.gameObject.SetActive(false); // the parent of this is the skillslot container, so disable it
            BattleTextLog.Instance.AddLine(m_battleUnit.skillSlots[m_skillSlotIndex].skill.skillName + " selected as skill.");
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
            if (m_turnTimerDisplay)
            {
                int timer = m_battleUnit.skillSlots[m_skillSlotIndex].turnTimer;
                if (timer == 0)
                    m_turnTimerDisplay.text = "";
                else
                    m_turnTimerDisplay.text = timer.ToString();
            }
        }
    }
}
