using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGSystem
{   
    public struct BattleMonsterID
    {
        public BattleMonsterID(int character, int monster)
        {
            m_character = character;
            m_monster = monster;
        }

        private int m_character;
        private int m_monster;

        public int character
        {
            get { return m_character; }
        }
        public int monster
        {
            get { return m_monster; }
        }

        public static bool operator ==(BattleMonsterID left, BattleMonsterID right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null))
                return false;
            if (ReferenceEquals(right, null))
                return false;
            return left.Equals(right);
        }
        public static bool operator !=(BattleMonsterID left, BattleMonsterID right) => !(left == right);
        public bool Equals(BattleMonsterID other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return character.Equals(other.character) && monster.Equals(other.monster);
        }
        public override bool Equals(object obj) => Equals((BattleMonsterID)obj);
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = character.GetHashCode();
                hashCode = (hashCode * 397) ^ monster.GetHashCode();
                return hashCode;
            }
        }
    }
    
    public class Action
    {
        public Action() { }
        public Action(BattleMonsterID user, BattleMonsterID[] targets, int skillIndex)
        {
            m_userID = user;
            m_targetIDs = targets;
            m_skillSlotIndex = skillIndex;
        }

        protected BattleMonsterID m_userID;
        protected BattleMonsterID[] m_targetIDs;
        protected int m_skillSlotIndex;
        protected bool[] m_buildState = new bool[3];

        public BattleMonsterID userID
        {
            get { return m_userID; }
        }
        public BattleMonsterID[] targetIDs
        {
            get { return m_targetIDs; }
        }
        public int skillSlotIndex
        {
            get { return m_skillSlotIndex; }
        }

        public bool[] buildState
        {
            get { return m_buildState; }
        }
        public bool isComplete
        {
            get { return m_buildState[0] && m_buildState[1] && m_buildState[2]; }
        }

        public void SetUser(BattleMonsterID userID)
        {
            m_userID = userID;
            m_buildState[0] = true;
        }
        public void SetSkillIndex(int skillSlotIndex)
        {
            m_skillSlotIndex = skillSlotIndex;
            m_buildState[1] = true;
        }
        public void SetTargets(BattleMonsterID[] targetIDs)
        {
            m_targetIDs = targetIDs;
            m_buildState[2] = true;
        }
    }
    
    public enum BattlePhase
    {
        Start,
        TurnStart,
        ChooseActions,
        Action,
        TurnEnd,
        End
    }
    
    public class BattleScene : MonoBehaviour
    {
        [SerializeField] protected BattleSceneUI m_battleSceneUIPrefab;
        protected BattleSceneUI m_battleSceneUI;
        
        /// <summary>
        /// Characters participating in the battle.
        /// </summary>
        [SerializeField] protected Character[] m_characters = new Character[GameSettings.CHARACTERS_PER_BATTLE];
        public Character[] characters
        {
            get { return m_characters; }
        }

        /// <summary>
        /// Row for each character, column for each BattleMonster.
        /// </summary>
        protected BattleMonster[,] m_battleMonsters;

        /// <summary>
        /// Number of turns passed since the start of battle.
        /// </summary>
        protected int m_turnNumber;
        public int turnNumber
        {
            get { return m_turnNumber; }
        }

        /// <summary>
        /// Phase of the current battle turn.
        /// </summary>
        protected BattlePhase m_currentPhase;

        /// <summary>
        /// The actions to be taken this turn.
        /// </summary>
        protected List<Action> m_turnActions = new List<Action>();

        /// <summary>
        /// The current action being built.
        /// </summary>
        protected Action m_currentAction;

        /// <summary>
        /// Gets the specified BattleMonster from the specified Character.
        /// </summary>
        /// <param name="character">Index representing the selected Character.</param>
        /// <param name="battleMonster">Index representing the selected BattleMonster.</param>
        /// <returns></returns>
        public BattleMonster GetBattleMonster(int character, int battleMonster)
        {
            return m_battleMonsters[character, battleMonster];
        }

        /// <summary>
        /// Gets the BattleMonster with a matching BattleMonsterID.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public BattleMonster GetBattleMonster(BattleMonsterID index)
        {
            return m_battleMonsters[index.character, index.monster];
        }

        // Start is called before the first frame update
        protected void Start()
        {
            m_currentPhase = BattlePhase.Start;
        }

        // Update is called once per frame
        protected void Update()
        {
            switch (m_currentPhase)
            {
                case BattlePhase.Start:
                    OnBattleStart();
                    m_currentPhase = BattlePhase.TurnStart;
                    break;
                case BattlePhase.TurnStart:
                    // check on turn start status effects for all monsters
                    break;
                case BattlePhase.ChooseActions:
                    if (WaitForActions())
                        m_currentPhase = BattlePhase.Action;
                    break;
                case BattlePhase.Action:
                    ProcessActions();
                    m_currentPhase = BattlePhase.TurnEnd;
                    break;
            }
        }

        protected void OnBattleStart()
        {
            // initialise BattleMonsters from characters
            m_battleMonsters = new BattleMonster[GameSettings.CHARACTERS_PER_BATTLE, GameSettings.MONSTERS_PER_PARTY];
            for (int cha = 0; cha < m_characters.Length; cha++)
            {
                for (int mon = 0; mon < m_characters[cha].monsters.Length; mon++)
                {
                    m_battleMonsters[cha, mon] = new BattleMonster(m_characters[cha].monsters[mon], new BattleMonsterID(cha, mon));
                }
            }

            // get characters and BattleMonsters ready for battle
            foreach (Character character in m_characters)
                character.ResetBattleCharacter();
            foreach (BattleMonster battleMonster in m_battleMonsters)
                battleMonster?.ResetBattleMonster();

            Debug.Log("Battle started between " + m_characters[0].characterName + " and " + m_characters[1].characterName + "!");

            // Instantiates the BattleSceneUI
            m_battleSceneUI = Instantiate(m_battleSceneUIPrefab);
            m_battleSceneUI.Initialise(this);
        }

        protected bool WaitForActions()
        {
            if (m_currentAction == null)
                m_currentAction = new Action();

            // build actions from UI object buttons
            // when action has user, skill id, and targets, action is complete and added to action list
            // when there is an action for every monster, this method returns true and the actions are done

            // action is completed
            if (m_currentAction.isComplete)
            {
                // add action to action list
            }
            
            return false;
        }

        public void AddUserSkillToAction(BattleMonsterID userID, int skillSlotIndex)
        {
            m_currentAction.SetUser(userID);
            m_currentAction.SetSkillIndex(skillSlotIndex);
        }

        public void AddTargetsToAction(BattleMonsterID[] targetIDs)
        {
            m_currentAction.SetTargets(targetIDs);
        }

        private void CreateAction(BattleMonsterID battleMonsterID, int skillSlotIndex)
        {
            
            m_turnActions.Add(new Action());
        }

        protected void ProcessActions()
        {
            foreach (Action action in m_turnActions)
            {
                BattleMonster user = GetBattleMonster(action.userID);

                // if the user is the target
                if (action.userID == action.targetIDs[0])
                {
                    Debug.Log(user.monsterName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on itself!");
                }
                // if the targets are other monsters
                else
                {
                    string targets = GetBattleMonster(action.targetIDs[0]).monsterName;
                    for (int i = 1; i < action.targetIDs.Length; i++)
                    {
                        if (action == m_turnActions.Last())
                            targets += " and " + GetBattleMonster(action.targetIDs[i]).monsterName;
                        else
                            targets += ", " + GetBattleMonster(action.targetIDs[i]).monsterName;

                    }
                    Debug.Log(user.monsterName + " attacks " + targets + 
                        " with " + user.skillSlots[action.skillSlotIndex].skill.skillName);
                }
            }
        }
    }
}
