using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RPGSystem
{   
    public class BattleSceneUI : ObjectUI
    {
        protected BattleScene m_battleScene;
        
        [Header("Character UI")]
        [SerializeField] private CharacterUI m_playerUIPrefab;
        [SerializeField] private CharacterUI m_enemyUIPrefab;
        protected CharacterUI[] m_characterUIArray;

        [Header("Unit UI")]
        [SerializeField] protected BattleUnitUI m_battleUnitUIPrefab;
        protected BattleUnitUI[] m_battleUnitUIArray;

        [Header("Skill Slot UI")]
        [SerializeField] protected SkillSlotUI m_skillSlotUIPrefab;
        protected SkillSlotUI[] m_skillSlotUIArray;

        public CharacterUI[] characterUIArray
        {
            get { return m_characterUIArray; }
        }
        public BattleUnitUI[] battleUnitUIArray
        {
            get { return m_battleUnitUIArray; }
        }
        public SkillSlotUI[] skillSlotUIArray
        {
            get { return m_skillSlotUIArray; }
        }

        public void Initialise(BattleScene battleScene)
        {
            m_battleScene = battleScene;

            int charactersPerBattle = GameSettings.CHARACTERS_PER_BATTLE;
            int unitsPerParty = GameSettings.UNITS_PER_PARTY;
            int maxSkillsPerUnit = GameSettings.MAX_SKILLS_PER_UNIT;

            m_characterUIArray = new CharacterUI[charactersPerBattle];
            m_battleUnitUIArray = new BattleUnitUI[charactersPerBattle * unitsPerParty];
            m_skillSlotUIArray = new SkillSlotUI[charactersPerBattle * unitsPerParty * maxSkillsPerUnit];

            // instantiate the character UIs
            for (int c = 0; c < m_characterUIArray.Length; c++)
            {
                m_characterUIArray[c] = Instantiate(c > 0 ? m_enemyUIPrefab : m_playerUIPrefab, transform);
                m_characterUIArray[c].Initialise(m_battleScene.characters[c], c);

                // Instantiate the BattleUnitUIs
                for (int m = 0; m < m_battleUnitUIArray.Length; m++)
                {
                    int unitIndex = c * unitsPerParty + m;
                    m_battleUnitUIArray[unitIndex] = Instantiate(m_battleUnitUIPrefab, m_characterUIArray[c].battleUnitUIContainer);
                    m_battleUnitUIArray[unitIndex].Initialise(m_battleScene, m_battleScene.GetBattleUnit(c,m), c > 0);

                    // Instantiate the skill slots
                    for (int s = 0; s < m_skillSlotUIArray.Length; s++)
                    {
                        int indexSkill = (c * unitsPerParty * maxSkillsPerUnit) + (m * maxSkillsPerUnit) + s;
                        m_skillSlotUIArray[indexSkill] = Instantiate(m_skillSlotUIPrefab, m_battleUnitUIArray[unitIndex].skillSlotUIContainer);
                        m_skillSlotUIArray[indexSkill].Initialise(m_battleScene, m_battleScene.GetBattleUnit(c, m), s % maxSkillsPerUnit);
                    }
                }
            }

            UpdateUI();
        }

        public override void UpdateUI()
        {
            foreach (CharacterUI ui in m_characterUIArray)
                ui.UpdateUI();
            foreach (BattleUnitUI ui in m_battleUnitUIArray)
                ui.UpdateUI();
            foreach (SkillSlotUI ui in m_skillSlotUIArray)
                ui.UpdateUI();
        }

        private void Update()
        {
            // DEBUG
            UpdateUI();
        }
    }
}

