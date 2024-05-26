using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGSystem
{   
    public struct BattleUnitID
    {
        public BattleUnitID(int character, int unit)
        {
            m_character = character;
            m_unit = unit;
        }

        private int m_character;
        private int m_unit;

        public int character
        {
            get { return m_character; }
        }
        public int unit
        {
            get { return m_unit; }
        }

        public static bool operator ==(BattleUnitID left, BattleUnitID right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null))
                return false;
            if (ReferenceEquals(right, null))
                return false;
            return left.Equals(right);
        }
        public static bool operator !=(BattleUnitID left, BattleUnitID right) => !(left == right);
        public bool Equals(BattleUnitID other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return character.Equals(other.character) && unit.Equals(other.unit);
        }
        public override bool Equals(object obj) => Equals((BattleUnitID)obj);
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = character.GetHashCode();
                hashCode = (hashCode * 397) ^ unit.GetHashCode();
                return hashCode;
            }
        }
    }
    
    public class Action
    {
        public Action() { isDirty = true; }
        public Action(BattleUnitID user, BattleUnitID target, int skillIndex)
        {
            m_userID = user;
            m_targetID = target;
            m_skillSlotIndex = skillIndex;
        }

        protected BattleUnitID m_userID;
        protected BattleUnitID m_targetID;
        protected int m_skillSlotIndex;
        protected int m_buildState;
        public bool isDirty;

        public BattleUnitID userID
        {
            get { return m_userID; }
        }
        public BattleUnitID targetID
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

        public void SetUser(BattleUnitID userID)
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
        public void SetTarget(BattleUnitID targetID)
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
        /// Row for each character, column for each BattleUnit.
        /// </summary>
        protected BattleUnit[,] m_battleUnits;

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
        /// Gets the specified BattleUnit from the specified Character.
        /// </summary>
        /// <param name="character">Index representing the selected Character.</param>
        /// <param name="battleUnit">Index representing the selected BattleUnit.</param>
        /// <returns></returns>
        public BattleUnit GetBattleUnit(int character, int battleUnit)
        {
            return m_battleUnits[character, battleUnit];
        }

        /// <summary>
        /// Gets the BattleUnit with a matching BattleUnitID.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public BattleUnit GetBattleUnit(BattleUnitID index)
        {
            return m_battleUnits[index.character, index.unit];
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
                    // check on turn start status effects for all units
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

        /// <summary>
        /// Initialises and prepares arrays of objects and Instantiates the UI.
        /// </summary>
        protected void OnBattleStart()
        {
            // get Characters and BattleUnits ready for battle
            foreach (Character character in m_characters)
            {
                character.ResetBattleCharacter();
            }
            m_battleUnits = new BattleUnit[GameSettings.CHARACTERS_PER_BATTLE, GameSettings.UNITS_PER_PARTY];
            for (int cha = 0; cha < m_characters.Length; cha++)
            {
                for (int mon = 0; mon < m_characters[cha].units.Length; mon++)
                {
                    m_battleUnits[cha, mon] = new BattleUnit(m_characters[cha].units[mon], new BattleUnitID(cha, mon));
                }
            }

            Debug.Log("Battle started between " + m_characters[0].characterName + " and " + m_characters[1].characterName + "!");

            // Instantiates the BattleSceneUI
            m_battleSceneUI = Instantiate(m_battleSceneUIPrefab);
            m_battleSceneUI.Initialise(this);
        }

        /// <summary>
        /// Build actions from UI object buttons.
        /// When action has user, skill id, and target, action is complete and added to action list.
        /// When there is an action for every unit, this method returns true and the actions are done.
        /// </summary>
        /// <returns>If all units have an action.</returns>
        protected bool WaitForActions()
        {
            // if null, create new from default constructor
            m_currentAction ??= new Action();

            // if the action has just been created or edited
            if (m_currentAction.isDirty)
            {
                // reset the state of all the UI
                foreach (BattleUnitUI ui in m_battleSceneUI.battleUnitUIArray)
                    ui.SetAsUnavailable();
                foreach (SkillSlotUI ui in m_battleSceneUI.skillSlotUIArray)
                    ui.SetAsAvailableSkill(false);

                // activate the relevant UI objects
                switch (m_currentAction.buildState)
                {    
                    // if has no user, skill, or target
                    case 0:
                        // activate the parties units to be selected as users
                        for (int m = 0; m < GameSettings.UNITS_PER_PARTY; m++)
                        {
                            // if the turn actions already contains that unit's action, then don't make it available as a user
                            if (m_turnActions.Find(action => action.userID == new BattleUnitID(m_currentAction.userID.character, m)) == null)
                            {
                                m_battleSceneUI.battleUnitUIArray[m_currentAction.userID.character * GameSettings.UNITS_PER_PARTY + m].SetAsAvailableUser();
                            }
                        }
                        break;
                    // if has a user, but no skill or target
                    case 1:
                        // activate the skill slots for the selected unit
                        for (int s = 0; s < GameSettings.MAX_SKILLS_PER_UNIT; s++)
                        {
                            m_battleSceneUI.skillSlotUIArray[m_currentAction.userID.character * GameSettings.CHARACTERS_PER_BATTLE + m_currentAction.userID.unit
                                * GameSettings.MAX_SKILLS_PER_UNIT + s].SetAsAvailableSkill(true);
                        }
                        break;
                    // if has user and skill, but no target
                    case 2:
                        // activate the units based on the TargetType of the skill
                        switch (GetBattleUnit(m_currentAction.userID).skillSlots[m_currentAction.skillSlotIndex].skill.target)
                        {
                            case TargetType.SingleParty:
                                // activate the parties units to be selected as targets
                                for (int m = 0; m < GameSettings.UNITS_PER_PARTY; m++)
                                    m_battleSceneUI.battleUnitUIArray[m_currentAction.userID.character * GameSettings.UNITS_PER_PARTY + m].SetAsAvailableTarget();
                                break;
                            case TargetType.Self:
                            case TargetType.EveryoneButSelf:
                            case TargetType.Everyone:
                            case TargetType.WholePartyButSelf:
                            case TargetType.WholeParty:
                                m_currentAction.SetTarget(m_currentAction.userID);
                                break;
                            case TargetType.AllEnemies:
                                // if the battle only has two characters in it, then just get the other character
                                if (GameSettings.CHARACTERS_PER_BATTLE == 2)
                                {
                                    m_currentAction.SetTarget(new BattleUnitID(m_currentAction.userID.character == 0 ? 1 : 0, 0));
                                    break;
                                }
                                goto case TargetType.SingleEnemy;
                            case TargetType.SingleEnemy:
                            case TargetType.AdjacentEnemies:
                                // activate all enemy units
                                for (int c = 0; c < GameSettings.CHARACTERS_PER_BATTLE; c++)
                                {
                                    // skip the user's character
                                    if (c == m_currentAction.userID.character)
                                        continue;
                                    for (int m = 0; m < GameSettings.UNITS_PER_PARTY; m++)
                                        m_battleSceneUI.battleUnitUIArray[c * GameSettings.UNITS_PER_PARTY + m].SetAsAvailableTarget();
                                }
                                break;
                        }
                        break;
                }

                // don't do this every frame
                m_currentAction.isDirty = false;
            }

            // then check if action is complete
            if (m_currentAction.buildState == 3)
            {
                m_turnActions.Add(m_currentAction);
                m_currentAction = null;

                // if all monters actions are completed, return true
                if (m_turnActions.Count == GameSettings.CHARACTERS_PER_BATTLE * GameSettings.UNITS_PER_PARTY)
                    return true;
            }

            return false;
        }

        public void AddUserToAction(BattleUnitID userID)
        {
            m_currentAction.SetUser(userID);
        }

        public void AddSkillToAction(int skillSlotIndex)
        {
            m_currentAction.SetSkillIndex(skillSlotIndex);
        }

        public void AddTargetToAction(BattleUnitID targetID)
        {
            m_currentAction.SetTarget(targetID);
        }

        protected void ProcessActions()
        {
            // sort action list by unit speed stats
            List<Action> orderedTurnActions = m_turnActions.OrderByDescending(action => GetBattleUnit(action.userID).speed).ToList();
            
            foreach (Action action in orderedTurnActions)
            {
                BattleUnit user = GetBattleUnit(action.userID);
                BattleUnit[] targets;

                // quick reference
                int userCharacter = action.userID.character;
                int userUnit = action.userID.unit;
                int targetCharacter = action.targetID.character;
                int targetUnit = action.targetID.unit;
                bool moreThanOneUnit = GameSettings.UNITS_PER_PARTY > 1;
                bool moreThanTwoUnits = GameSettings.UNITS_PER_PARTY > 2;

                // position: 0 = left, MAX = right, else is in middle somewhere
                int userPosition = (userCharacter * GameSettings.UNITS_PER_PARTY + targetUnit) % GameSettings.UNITS_PER_PARTY;
                int targetPosition = (targetCharacter * GameSettings.UNITS_PER_PARTY + targetUnit) % GameSettings.UNITS_PER_PARTY;

                // switch uses this to hold a value
                int buffer;
                switch (user.skillSlots[action.skillSlotIndex].skill.target)
                {
                    case TargetType.SingleEnemy:
                    case TargetType.SingleParty:
                        targets = new BattleUnit[1];
                        targets[0] = GetBattleUnit(action.targetID);
                        Debug.Log(user.unitNickname + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + 
                            " on " + GetBattleUnit(action.targetID).unitNickname + "!");
                        break;
                    case TargetType.Self:
                        targets = new BattleUnit[1];
                        targets[0] = user;
                        Debug.Log(user.unitNickname + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on itself!");
                        break;
                    case TargetType.AdjacentEnemies:
                        if (moreThanOneUnit)
                        {
                            if (targetPosition == 0 || targetPosition == GameSettings.UNITS_PER_PARTY - 1)
                            {
                                targets = new BattleUnit[2];
                                targets[0] = GetBattleUnit(action.targetID);
                                targets[1] = GetBattleUnit(targetCharacter, targetUnit + (targetPosition == 0 ? 1 : -1));
                                Debug.Log(user.unitNickname + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on " +
                                    GetBattleUnit(action.targetID).unitNickname + " and " + GetBattleUnit(targetCharacter, targetUnit + 1).unitNickname + "!");
                            }
                            else if (moreThanTwoUnits)
                            {
                                targets = new BattleUnit[3];
                                targets[0] = GetBattleUnit(targetCharacter, targetUnit - 1);
                                targets[1] = GetBattleUnit(action.targetID);
                                targets[2] = GetBattleUnit(targetCharacter, targetUnit + 1);
                                Debug.Log(user.unitNickname + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on " +
                                    GetBattleUnit(targetCharacter, targetUnit - 1).unitNickname + ", " + GetBattleUnit(action.targetID).unitNickname + " and  " +
                                    GetBattleUnit(targetCharacter, targetUnit + 1).unitNickname + "!");
                            }
                            break;
                        }
                        goto case TargetType.SingleParty;
                    case TargetType.AllEnemies:
                        targets = new BattleUnit[GameSettings.UNITS_PER_PARTY];
                        for (int m = 0; m < GameSettings.UNITS_PER_PARTY; m++)
                        {
                            targets[m] = GetBattleUnit(targetCharacter, m);
                        }
                        Debug.Log(user.unitNickname + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on the entire enemy party!");
                        break;
                    case TargetType.WholePartyButSelf:
                        targets = new BattleUnit[GameSettings.UNITS_PER_PARTY - 1];
                        buffer = 0;
                        for (int m = 0; m < GameSettings.UNITS_PER_PARTY; m++)
                        {
                            if (m_battleUnits[userCharacter, m].battleID != action.userID)
                                targets[m - buffer] = GetBattleUnit(userCharacter, m);
                            else
                                buffer = -1;
                        }
                        Debug.Log(user.unitNickname + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on its party members!");
                        break;
                    case TargetType.WholeParty:
                        targets = new BattleUnit[GameSettings.UNITS_PER_PARTY];
                        for (int m = 0; m < GameSettings.UNITS_PER_PARTY; m++)
                        {
                            targets[m] = GetBattleUnit(userCharacter, m);
                        }
                        Debug.Log(user.unitNickname + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on its whole party!");
                        break;
                    case TargetType.EveryoneButSelf:
                        targets = new BattleUnit[GameSettings.CHARACTERS_PER_BATTLE * GameSettings.UNITS_PER_PARTY - 1];
                        buffer = 0;
                        for (int c = 0; c < GameSettings.CHARACTERS_PER_BATTLE; c++)
                        {
                            for (int m = 0; m < GameSettings.UNITS_PER_PARTY; m++)
                            {
                                if (m_battleUnits[c, m].battleID != action.userID)
                                    targets[c * GameSettings.UNITS_PER_PARTY + m + buffer] = m_battleUnits[c, m];
                                else
                                    buffer = -1;
                            }
                        }
                        Debug.Log(user.unitNickname + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on everyone else!");
                        break;
                    case TargetType.Everyone:
                        targets = new BattleUnit[GameSettings.CHARACTERS_PER_BATTLE * GameSettings.UNITS_PER_PARTY];
                        for (int c = 0; c < GameSettings.CHARACTERS_PER_BATTLE; c++)
                        {
                            for (int m = 0; m < GameSettings.UNITS_PER_PARTY; m++)
                            {
                                targets[c * GameSettings.CHARACTERS_PER_BATTLE + m] = m_battleUnits[c, m];
                            }
                        }
                        Debug.Log(user.unitNickname + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on everyone!");
                        break;
                }
            }
        }
    }
}
