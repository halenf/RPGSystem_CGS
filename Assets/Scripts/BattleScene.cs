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
        [SerializeField] private BattleSceneUI m_battleSceneUIPrefab;
        
        /// <summary>
        /// Characters participating in the battle
        /// </summary>
        [SerializeField] protected Character[] m_characters = new Character[GameSettings.CHARACTERS_PER_BATTLE];
        public Character[] characters
        {
            get { return m_characters; }
        }

        /// <summary>
        /// Row for each character, column for each battle monster.
        /// </summary>
        protected BattleMonster[,] m_battleMonsters;

        // Battle details
        protected int m_turnNumber;
        public int turnNumber
        {
            get { return m_turnNumber; }
        }
        protected BattleState m_state;

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

        /// <summary>
        /// Gets the specified battle monsters from the battle monsters array.
        /// </summary>
        /// <param name="character">Index representing the selected character.</param>
        /// <param name="battleMonster">Index representing the selected battle monster.</param>
        /// <returns></returns>
        public BattleMonster GetBattleMonster(int character, int battleMonster)
        {
            return m_battleMonsters[character, battleMonster];
        }
        
        // Start is called before the first frame update
        protected void Start()
        {
            // initialise battle monsters from characters
            m_battleMonsters = new BattleMonster[GameSettings.CHARACTERS_PER_BATTLE, GameSettings.MONSTERS_PER_PARTY];
            for (int row = 0; row < m_characters.Length; row++)
            {
                for (int col = 0; col < m_characters[row].monsters.Length; col++)
                {
                    m_battleMonsters[row, col] = new BattleMonster(m_characters[row].monsters[col]);
                }
            }

            // Instantiates the BattleSceneUI
            BattleSceneUI battleUI = Instantiate(m_battleSceneUIPrefab);
            battleUI.Initialise(m_characters);
            
            OnBattleStart();
        }

        // Update is called once per frame
        protected void Update()
        {
            switch (m_state)
            {
                case BattleState.Start:
                    //fart
                    break;
            }
        }

        protected void OnBattleStart()
        {
            m_state = BattleState.Start;

            // get characters and battle monsters ready for battle
            foreach (Character character in m_characters)
                character.ResetBattleCharacter();
            foreach (BattleMonster battleMonster in m_battleMonsters)
                battleMonster.ResetBattleMonster();
        }
    }
}
