using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RPGSystem
{   
    public class BattleSceneUI : ObjectUI
    {
        public CharacterUI m_characterUIPrefab;
        [SerializeField] private CharacterUI[] m_characterUIArray;

        public CharacterUI characterUI
        {
            get { return m_characterUIArray[0]; }
        }

        public void Initialise(Character[] characters)
        {
            int characterCount = GameManager.current.gameSettings.charactersPerBattle;
            m_characterUIArray = new CharacterUI[characterCount];
            for (int i = 0; i < characterCount; i++)
            {
                m_characterUIArray[i] = Instantiate(m_characterUIPrefab, transform);
                m_characterUIArray[i].Initialise(characters[i]);
            }

            UpdateUI();
        }

        public override void UpdateUI()
        {
            foreach (CharacterUI ui in m_characterUIArray)
                ui.UpdateUI();
        }
    }
}

