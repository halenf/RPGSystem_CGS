using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{   
    public enum BattleState
    {
        Start,
        TurnStart,
        Action,
        TurnEnd,
        End
    }
    
    public class BattleScene : MonoBehaviour
    {
        private Character[] m_characters = new Character[2];
        private int m_turnNumber;
        public int turnNumber
        {
            get { return m_turnNumber; }
        }
        private BattleState m_state;

        /// <summary>
        /// Gets the specified monster from the first or second character in the battle.
        /// </summary>
        /// <param name="character">Index representing the selected character.</param>
        /// <param name="monster">Index representing the selected monster.</param>
        /// <returns></returns>
        public Monster GetMonster(int character, int monster)
        {
            return m_characters[character].monsters[monster];
        }
        
        // Start is called before the first frame update
        void Start()
        {
            // Instantiates a BattleSceneUI object here
            
            m_state = BattleState.Start;
            OnBattleStart();
        }

        // Update is called once per frame
        void Update()
        {
            switch (m_state)
            {
                case BattleState.Start:
                    
                    break;
            }
        }

        private void OnBattleStart()
        {

        }
    }
}
