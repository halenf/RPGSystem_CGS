using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RPGSystem
{   
    public class BattleSceneUI : ObjectUI
    {
        protected BattleScene m_battleScene;
        
        [Header("Character UI")]
        [SerializeField] private CharacterUI m_playerCharacterUIPrefab;
        [SerializeField] private CharacterUI m_enemyCharacterUIPrefab;
        protected CharacterUI[] m_characterUIArray;

        [Header("Unit UI")]
        [SerializeField] protected BattleUnitUI m_playerUnitUIPrefab;
        [SerializeField] protected BattleUnitUI m_enemyUnitUIPrefab;
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

            m_characterUIArray = new CharacterUI[m_battleScene.characters.Length];
            m_battleUnitUIArray = new BattleUnitUI[charactersPerBattle * unitsPerParty];
            m_skillSlotUIArray = new SkillSlotUI[charactersPerBattle * unitsPerParty * maxSkillsPerUnit];

            // instantiate the character UIs
            for (int c = 0; c < m_characterUIArray.Length; c++)
            {
                m_characterUIArray[c] = Instantiate(c == 0 ? m_playerCharacterUIPrefab : m_enemyCharacterUIPrefab, transform);
                m_characterUIArray[c].Initialise(m_battleScene.characters[c], c);

                // Instantiate the BattleUnitUIs
                for (int u = 0; u < GameSettings.UNITS_PER_PARTY; u++)
                {
                    if (m_battleScene.GetBattleUnit(c, u) != null)
                    {
                        int unitUIIndex = c * unitsPerParty + u;
                        m_battleUnitUIArray[unitUIIndex] = Instantiate(c == 0 ? m_playerUnitUIPrefab : m_enemyUnitUIPrefab, m_characterUIArray[c].battleUnitUIContainer);
                        m_battleUnitUIArray[unitUIIndex].Initialise(m_battleScene, m_battleScene.GetBattleUnit(c,u), c > 0);

                        // Instantiate the skill slots for the Unit
                        for (int s = 0; s < GameSettings.MAX_SKILLS_PER_UNIT; s++)
                        {
                            if (m_battleScene.GetBattleUnit(c, u).skillSlots.ElementAtOrDefault(s) != null)
                            {
                                int skillUIIndex = (c * unitsPerParty * maxSkillsPerUnit) + (u * maxSkillsPerUnit) + s;
                                m_skillSlotUIArray[skillUIIndex] = Instantiate(m_skillSlotUIPrefab, m_battleUnitUIArray[unitUIIndex].skillSlotUIContainer);
                                m_skillSlotUIArray[skillUIIndex].Initialise(m_battleScene, m_battleScene.GetBattleUnit(c, u), s % maxSkillsPerUnit);
                            }
                        }
                    }
                }
            }

            UpdateUI();
        }

        public override void UpdateUI()
        {
            foreach (CharacterUI ui in m_characterUIArray)
                ui.UpdateUI();
            for (int u = 0; u < m_battleUnitUIArray.Length; u++)
                if (m_battleUnitUIArray[u] != null)
                    m_battleUnitUIArray[u].UpdateUI();
            for (int s = 0; s < m_skillSlotUIArray.Length; s++)
                if (m_skillSlotUIArray[s] != null)
                    m_skillSlotUIArray[s].UpdateUI();
        }

        private void Update()
        {
            // DEBUG
            UpdateUI();
        }
    }
}

