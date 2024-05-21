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
        public Action() { isDirty = true; }
        public Action(BattleMonsterID user, BattleMonsterID target, int skillIndex)
        {
            m_userID = user;
            m_targetID = target;
            m_skillSlotIndex = skillIndex;
        }

        protected BattleMonsterID m_userID;
        protected BattleMonsterID m_targetID;
        protected int m_skillSlotIndex;
        protected int m_buildState;
        public bool isDirty;

        public BattleMonsterID userID
        {
            get { return m_userID; }
        }
        public BattleMonsterID targetID
        {
            get { return m_targetID; }
        }
        public int skillSlotIndex
        {
            get { return m_skillSlotIndex; }
        }

        public int buildState
        {
            get { return m_buildState; }
        }

        public void SetUser(BattleMonsterID userID)
        {
            m_userID = userID;
            m_buildState = 1;
            isDirty = true;
        }
        public void SetSkillIndex(int skillSlotIndex)
        {
            m_skillSlotIndex = skillSlotIndex;
            m_buildState = 2;
            isDirty = true;
        }
        public void SetTarget(BattleMonsterID targetID)
        {
            m_targetID = targetID;
            m_buildState = 3;
            isDirty = true;
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

            // get Characters and BattleMonsters ready for battle
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

            if (m_currentAction.isDirty)
            {
                // reset the state of all the UI
                foreach (BattleMonsterUI ui in m_battleSceneUI.battleMonsterUIArray)
                    ui.SetAsUnavailable();
                foreach (SkillSlotUI ui in m_battleSceneUI.skillSlotUIArray)
                    ui.SetAsAvailableSkill(false);

                // activate the relevant UI objects
                switch (m_currentAction.buildState)
                {    
                    // if has no user, skill, or target
                    case 0:
                        // activate the parties monsters to be selected as users
                        for (int m = 0; m < GameSettings.MONSTERS_PER_PARTY; m++)
                        {
                            // if the turn actions already contains that monster's action, then don't make it available as a user
                            if (m_turnActions.Find(action => action.userID == new BattleMonsterID(0, m)) == null)
                                m_battleSceneUI.battleMonsterUIArray[m].SetAsAvailableUser();
                        }
                        break;
                    // if has a user, but no skill or target
                    case 1:
                        // activate the skill slots for the selected monster
                        for (int s = 0; s < GameSettings.MAX_SKILLS_PER_MONSTER; s++)
                        {
                            m_battleSceneUI.skillSlotUIArray[m_currentAction.userID.monster * GameSettings.MAX_SKILLS_PER_MONSTER + s].SetAsAvailableSkill(true);
                        }
                        break;
                    // if has user and skill, but no target
                    case 2:
                        // activate the monsters based on the TargetType of the skill
                        switch (GetBattleMonster(m_currentAction.userID).skillSlots[m_currentAction.skillSlotIndex].skill.target)
                        {
                            case TargetType.Self: //done
                            case TargetType.EveryoneButSelf: //done
                            case TargetType.Everyone: //done
                            case TargetType.AllEnemies:
                            case TargetType.WholePartyButSelf:
                            case TargetType.WholeParty:
                                m_currentAction.SetTarget(m_currentAction.userID);
                                break;
                            
                                //break;
                        }
                        break;
                    // action is completed
                    case 3:
                        m_turnActions.Add(m_currentAction);
                        m_currentAction = null;

                        // if all monters actions are completed, return true
                        if (m_turnActions.Count == GameSettings.CHARACTERS_PER_BATTLE * GameSettings.MONSTERS_PER_PARTY)
                            return true;
                        break;
                }

                // don't do this every frame
                m_currentAction.isDirty = false;
            }

            // build actions from UI object buttons
            // when action has user, skill id, and targets, action is complete and added to action list
            // when there is an action for every monster, this method returns true and the actions are done

            
            return false;
        }

        public void AddUserToAction(BattleMonsterID userID)
        {
            m_currentAction.SetUser(userID);
        }

        public void AddSkillToAction(int skillSlotIndex)
        {
            m_currentAction.SetSkillIndex(skillSlotIndex);
        }

        public void AddTargetToAction(BattleMonsterID targetID)
        {
            m_currentAction.SetTarget(targetID);
        }

        protected void ProcessActions()
        {
            // sort action list by monster speed stats
            List<Action> orderedTurnActions = m_turnActions.OrderByDescending(action => GetBattleMonster(action.userID).speed).ToList();
            
            foreach (Action action in orderedTurnActions)
            {
                BattleMonster user = GetBattleMonster(action.userID);
                BattleMonster[] targets;

                int userCharacter = action.userID.character;
                int userMonster = action.userID.monster;
                int targetCharacter = action.targetID.character;
                int targetMonster = action.targetID.monster;
                int userPosition = (userCharacter * GameSettings.MONSTERS_PER_PARTY + targetMonster) % GameSettings.MONSTERS_PER_PARTY;
                int targetPosition = (targetCharacter * GameSettings.MONSTERS_PER_PARTY + targetMonster) % GameSettings.MONSTERS_PER_PARTY;

                int buffer;
                switch (user.skillSlots[action.skillSlotIndex].skill.target)
                {
                    case TargetType.SingleEnemy:
                        continue;
                    case TargetType.SingleParty:
                        targets = new BattleMonster[1];
                        targets[0] = GetBattleMonster(action.targetID);
                        Debug.Log(user.monsterName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + 
                            " on " + GetBattleMonster(action.targetID).monsterName + "!");
                        break;
                    case TargetType.Self:
                        targets = new BattleMonster[1];
                        targets[0] = user;
                        Debug.Log(user.monsterName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on itself!");
                        break;
                    case TargetType.AdjacentEnemies:
                        // position: 0 = left, MAX = right, else is in middle somewhere
                        if (targetPosition == 0 || targetPosition == GameSettings.MONSTERS_PER_PARTY - 1)
                        {
                            targets = new BattleMonster[2];
                            targets[0] = GetBattleMonster(action.targetID);
                            targets[1] = GetBattleMonster(targetCharacter, targetMonster + (targetPosition == 0 ? 1 : -1));
                            Debug.Log(user.monsterName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on " +
                                GetBattleMonster(action.targetID).monsterName + " and " + GetBattleMonster(targetCharacter, targetMonster + 1).monsterName + "!");
                        }
                        else
                        {
                            targets = new BattleMonster[3];
                            targets[0] = GetBattleMonster(targetCharacter, targetMonster - 1);
                            targets[1] = GetBattleMonster(action.targetID);
                            targets[2] = GetBattleMonster(targetCharacter, targetMonster + 1);
                            Debug.Log(user.monsterName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on " +
                                GetBattleMonster(targetCharacter, targetMonster - 1).monsterName + ", " + GetBattleMonster(action.targetID).monsterName + " and  " +
                                GetBattleMonster(targetCharacter, targetMonster + 1).monsterName + "!");
                        }
                        break;
                    case TargetType.AllEnemies:
                        targets = new BattleMonster[3];
                        targets[0] = GetBattleMonster(targetCharacter, targetMonster - 1);
                        targets[1] = GetBattleMonster(action.targetID);
                        targets[2] = GetBattleMonster(targetCharacter, targetMonster + 1);
                        Debug.Log(user.monsterName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on " +
                            GetBattleMonster(targetCharacter, targetMonster - 1).monsterName + ", " + GetBattleMonster(action.targetID).monsterName + " and  " +
                            GetBattleMonster(targetCharacter, targetMonster + 1).monsterName + "!");
                        break;
                    case TargetType.WholePartyButSelf:
                        targets = new BattleMonster[GameSettings.MONSTERS_PER_PARTY - 1];
                        buffer = 0;
                        for (int m = 0; m < GameSettings.MONSTERS_PER_PARTY; m++)
                        {
                            if (m_battleMonsters[userCharacter, m].battleID != action.userID)
                                targets[userCharacter * GameSettings.CHARACTERS_PER_BATTLE + m + buffer] = m_battleMonsters[userCharacter, m];
                            else
                                buffer = -1;
                        }
                        Debug.Log(user.monsterName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on " +
                                GetBattleMonster(targetCharacter, targetMonster - 1).monsterName + " and " + GetBattleMonster(targetCharacter, targetMonster + 1).monsterName + "!");
                        break;
                    case TargetType.WholeParty:
                        targets = new BattleMonster[3];
                        targets[0] = GetBattleMonster(targetCharacter, targetMonster - 1);
                        targets[1] = user;
                        targets[2] = GetBattleMonster(targetCharacter, targetMonster + 1);
                        Debug.Log(user.monsterName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on " +
                                    GetBattleMonster(targetCharacter, targetMonster - 1).monsterName + ", " + GetBattleMonster(targetCharacter, targetMonster + 1).monsterName +
                                    " and itself!");
                        break;
                    case TargetType.EveryoneButSelf:
                        targets = new BattleMonster[GameSettings.CHARACTERS_PER_BATTLE * GameSettings.MONSTERS_PER_PARTY - 1];
                        buffer = 0;
                        for (int c = 0; c < GameSettings.CHARACTERS_PER_BATTLE; c++)
                        {
                            for (int m = 0; m < GameSettings.MONSTERS_PER_PARTY; m++)
                            {
                                if (m_battleMonsters[c, m].battleID != action.userID)
                                    targets[c * GameSettings.MONSTERS_PER_PARTY + m + buffer] = m_battleMonsters[c, m];
                                else
                                    buffer = -1;
                            }
                        }
                        Debug.Log(user.monsterName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on everyone else!");
                        break;
                    case TargetType.Everyone:
                        targets = new BattleMonster[GameSettings.CHARACTERS_PER_BATTLE * GameSettings.MONSTERS_PER_PARTY];
                        for (int c = 0; c < GameSettings.CHARACTERS_PER_BATTLE; c++)
                        {
                            for (int m = 0; m < GameSettings.MONSTERS_PER_PARTY; m++)
                            {
                                targets[c * GameSettings.CHARACTERS_PER_BATTLE + m] = m_battleMonsters[c, m];
                            }
                        }
                        Debug.Log(user.monsterName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on everyone!");
                        break;
                }
            }
        }
    }
}
