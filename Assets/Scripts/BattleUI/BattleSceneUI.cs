using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RPGSystem
{   
    public class BattleSceneUI : ObjectUI
    {
        [SerializeField] private CharacterUI[] m_characterUIList = new CharacterUI[2];

        public CharacterUI characterUI
        {
            get { return m_characterUIList[0]; }
        }

        public void Initialise(Character character1, Character character2)
        {
            m_characterUIList = GetComponentsInChildren<CharacterUI>();
            // give the Character references to the CharacterUI objects
            m_characterUIList[0].character = character1;
            //m_characterUIList[1].character = character2;

            foreach (CharacterUI ui in m_characterUIList)
                ui.Initialise();

            UpdateUI();
        }

        public override void UpdateUI()
        {
            foreach (CharacterUI ui in m_characterUIList)
                ui.UpdateUI();
        }
    }
}

