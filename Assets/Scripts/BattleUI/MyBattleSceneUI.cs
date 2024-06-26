using System.Linq;
using UnityEngine;

namespace RPGSystem
{
    public class MyBattleSceneUI : ObjectUI
    {
        private MyBattleScene m_battleScene;
        
        [Header("Character UI")]
        [SerializeField] private MyCharacterUI m_playerCharacterUIPrefab;
        [SerializeField] private MyCharacterUI m_enemyCharacterUIPrefab;
        private MyCharacterUI[] m_characterUIArray;

        [Header("Unit UI")]
        [SerializeField] private MyBattleUnitUI m_playerUnitUIPrefab;
        [SerializeField] private MyBattleUnitUI m_enemyUnitUIPrefab;
        private MyBattleUnitUI[] m_battleUnitUIArray;

        [Header("Skill Slot UI")]
        [SerializeField] private SkillSlotUI m_skillSlotUIPrefab;
        private SkillSlotUI[] m_skillSlotUIArray;

        public MyCharacterUI[] characterUIArray
        {
            get { return m_characterUIArray; }
        }
        public MyBattleUnitUI[] battleUnitUIArray
        {
            get { return m_battleUnitUIArray; }
        }
        public SkillSlotUI[] skillSlotUIArray
        {
            get { return m_skillSlotUIArray; }
        }

        public void Initialise(MyBattleScene battleScene)
        {
            m_battleScene = battleScene;

            int charactersPerBattle = GameSettings.CHARACTERS_PER_BATTLE;
            int unitsPerParty = GameSettings.UNITS_PER_PARTY;
            int maxSkillsPerUnit = GameSettings.MAX_SKILLS_PER_UNIT;

            m_characterUIArray = new MyCharacterUI[m_battleScene.characters.Length];
            m_battleUnitUIArray = new MyBattleUnitUI[charactersPerBattle * unitsPerParty];
            m_skillSlotUIArray = new SkillSlotUI[charactersPerBattle * unitsPerParty * maxSkillsPerUnit];

            // instantiate the character UIs
            for (int c = 0; c < m_characterUIArray.Length; c++)
            {
                m_characterUIArray[c] = Instantiate(c == 0 ? m_playerCharacterUIPrefab : m_enemyCharacterUIPrefab, transform);
                m_characterUIArray[c].Initialise(m_battleScene.characters[c] as MyCharacter, c);

                // Instantiate the BattleUnitUIs
                for (int u = 0; u < GameSettings.UNITS_PER_PARTY; u++)
                {
                    if (m_battleScene.GetBattleUnit(c, u) != null)
                    {
                        int unitUIIndex = c * unitsPerParty + u;
                        m_battleUnitUIArray[unitUIIndex] = Instantiate(c == 0 ? m_playerUnitUIPrefab : m_enemyUnitUIPrefab, m_characterUIArray[c].battleUnitUIContainer);
                        m_battleUnitUIArray[unitUIIndex].Initialise(m_battleScene, m_battleScene.GetBattleUnit(c,u) as MyBattleUnit, c > 0);

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
            foreach (MyCharacterUI ui in m_characterUIArray)
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

