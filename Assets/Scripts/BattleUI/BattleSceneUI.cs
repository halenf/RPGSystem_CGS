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

        [Header("Monster UI")]
        [SerializeField] protected BattleMonsterUI m_battleMonsterUIPrefab;
        protected BattleMonsterUI[] m_battleMonsterUIArray;

        [Header("Skill Slot UI")]
        [SerializeField] protected SkillSlotUI m_skillSlotUIPrefab;
        protected SkillSlotUI[] m_skillSlotUIArray;

        public CharacterUI characterUI
        {
            get { return m_characterUIArray[0]; }
        }

        public void Initialise(BattleScene battleScene)
        {
            m_battleScene = battleScene;

            int charactersPerBattle = GameSettings.CHARACTERS_PER_BATTLE;
            int monstersPerParty = GameSettings.MONSTERS_PER_PARTY;
            int maxSkillsPerMonster = GameSettings.MAX_SKILLS_PER_MONSTER;

            m_characterUIArray = new CharacterUI[charactersPerBattle];
            m_battleMonsterUIArray = new BattleMonsterUI[charactersPerBattle * monstersPerParty];
            m_skillSlotUIArray = new SkillSlotUI[charactersPerBattle * monstersPerParty * maxSkillsPerMonster];

            // instantiate the character UIs
            for (int c = 0; c < m_characterUIArray.Length; c++)
            {
                m_characterUIArray[c] = Instantiate(c > 0 ? m_enemyUIPrefab : m_playerUIPrefab, transform);
                m_characterUIArray[c].Initialise(m_battleScene.characters[c], c);

                // Instantiate the BattleMonsterUIs
                for (int m = 0; m < m_battleMonsterUIArray.GetLength(1); m++)
                {
                    m_battleMonsterUIArray[(c * monstersPerParty) + m] = Instantiate(m_battleMonsterUIPrefab, m_characterUIArray[c].transform);
                    m_battleMonsterUIArray[(c * monstersPerParty) + m].Initialise(m_battleScene, m_battleScene.GetBattleMonster(c,m), c > 0 ? true : false);

                    for (int s = 0; s < m_skillSlotUIArray.GetLength(2); s++)
                    {
                        m_skillSlotUIArray[(c * monstersPerParty * maxSkillsPerMonster) + (m * maxSkillsPerMonster) + s] = Instantiate(m_skillSlotUIPrefab, m_battleMonsterUIArray[m].transform);
                    }
                }
            }

            UpdateUI();
        }

        public override void UpdateUI()
        {
            foreach (CharacterUI ui in m_characterUIArray)
                ui.UpdateUI();
            foreach (BattleMonsterUI ui in m_battleMonsterUIArray)
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

