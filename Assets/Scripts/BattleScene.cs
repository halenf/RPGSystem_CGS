using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGSystem
{   
    public readonly struct BattleUnitID
    {
        public BattleUnitID(int character, int unit)
        {
            m_character = character;
            m_battleUnit = unit;
        }

        private readonly int m_character;
        private readonly int m_battleUnit;

        public int character
        {
            get { return m_character; }
        }
        public int battleUnit
        {
            get { return m_battleUnit; }
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
            return character.Equals(other.character) && battleUnit.Equals(other.battleUnit);
        }
        public override bool Equals(object obj) => Equals((BattleUnitID)obj);
        public override int GetHashCode()
        {
            return System.HashCode.Combine(character, battleUnit);
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
        [SerializeField] protected Character[] m_characters;
        public Character[] characters
        {
            get { return m_characters; }
        }

        /// <summary>
        /// Row for each character, column for each BattleUnit.
        /// </summary>
        protected BattleUnit[] m_battleUnits;

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
        /// <param name="unit">Index representing the selected BattleUnit.</param>
        /// <returns></returns>
        public BattleUnit GetBattleUnit(int character, int unit)
        {
            return m_battleUnits[character * GameSettings.UNITS_PER_PARTY + unit];
        }

        /// <summary>
        /// Gets the BattleUnit with a matching BattleUnitID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BattleUnit GetBattleUnit(BattleUnitID id)
        {
            return m_battleUnits[id.character * GameSettings.UNITS_PER_PARTY + id.battleUnit];
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
                    m_currentPhase = BattlePhase.ChooseActions;
                    break;
                case BattlePhase.ChooseActions:
                    if (WaitForActions())
                        m_currentPhase = BattlePhase.Action;
                    break;
                case BattlePhase.Action:
                    ProcessActions();
                    m_currentPhase = BattlePhase.TurnEnd;
                    break;
                case BattlePhase.TurnEnd:
                    m_currentPhase = BattlePhase.TurnStart;
                    break;
                case BattlePhase.End:
                    break;
            }
        }

        /// <summary>
        /// Initialises and prepares arrays of objects and Instantiates the UI.
        /// </summary>
        protected void OnBattleStart()
        {
            Debug.Log("Battle started between " + m_characters[0].characterName + " and " + m_characters[1].characterName + "!");

            // get Characters and BattleUnits ready for battle
            foreach (Character character in m_characters)
            {
                character.InitialiseForBattle();
                foreach (Unit unit in character.units)
                {
                    unit.InitialiseForBattle();
                }
            }
            m_battleUnits = new BattleUnit[m_characters.Length * GameSettings.UNITS_PER_PARTY];
            for (int c = 0; c < m_characters.Length; c++)
            {
                for (int u = 0; u < m_characters[c].units.Length; u++)
                {
                    int index = c * GameSettings.UNITS_PER_PARTY + u;
                    m_battleUnits[index] = new BattleUnit(m_characters[c].units[u], new BattleUnitID(c, u));
                    m_battleUnits[index].ResetBattleUnit();
                    Debug.Log(m_characters[c].characterName + " sends out " + GetBattleUnit(c, u).displayName + "!");
                }
            }

            // Instantiates the BattleSceneUI
            m_battleSceneUI = Instantiate(m_battleSceneUIPrefab);
            m_battleSceneUI.Initialise(this);
        }

        /// <summary>
        /// Build actions from UI object buttons.
        /// When action has user, skill id, and targets, action is complete and added to action list.
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
                for (int u = 0; u < m_battleSceneUI.battleUnitUIArray.Length; u++)
                    if (m_battleSceneUI.battleUnitUIArray[u] != null)
                        m_battleSceneUI.battleUnitUIArray[u].SetAsUnavailable();
                for (int s = 0; s < m_battleSceneUI.skillSlotUIArray.Length; s++)
                    if (m_battleSceneUI.skillSlotUIArray[s] != null)
                        m_battleSceneUI.skillSlotUIArray[s].SetAsAvailableSkill(false);

                // activate the relevant UI objects
                switch (m_currentAction.buildState)
                {    
                    // if has no user, skill, or targets
                    case 0:
                        Debug.Log("Choose one of your Units.");
                        // activate the parties units to be selected as users
                        for (int u = 0; u < m_characters[0].units.Length; u++)
                        {
                            // if the turn actions already contains that unit's action, then don't make it available as a user
                            if (m_turnActions.Find(action => action.userID == new BattleUnitID(0, u)) == null)
                                m_battleSceneUI.battleUnitUIArray[u].SetAsAvailableUser();
                        }
                        break;
                    // if has a user, but no skill or targets
                    case 1:
                        Debug.Log("Choose a skill for " + GetBattleUnit(m_currentAction.userID).displayName + " to use.");
                        // activate the skill slots for the selected unit
                        for (int s = 0; s < GetBattleUnit(0, m_currentAction.userID.battleUnit).skillSlots.Count; s++)
                        {
                            int skillIndex = m_currentAction.userID.battleUnit * GameSettings.MAX_SKILLS_PER_UNIT + s;
                            m_battleSceneUI.skillSlotUIArray[skillIndex].SetAsAvailableSkill(true);
                        }
                        break;
                    // if has user and skill, but no targets
                    case 2:
                        Debug.Log("Choose the target for " + GetBattleUnit(m_currentAction.userID).skillSlots[m_currentAction.skillSlotIndex].skill.skillName + ".");
                        // activate the units based on the TargetType of the skill
                        switch (GetBattleUnit(m_currentAction.userID).skillSlots[m_currentAction.skillSlotIndex].skill.targets)
                        {
                            case TargetType.SingleParty:
                                // activate the parties units to be selected as targets
                                for (int u = 0; u < m_characters[0].units.Length; u++)
                                    m_battleSceneUI.battleUnitUIArray[u].SetAsAvailableTarget();
                                break;
                            case TargetType.Self:
                            case TargetType.EveryoneButSelf:
                            case TargetType.Everyone:
                            case TargetType.WholePartyButSelf:
                            case TargetType.WholeParty:
                                // Target Types where the Skill's target is irrelevant
                                m_currentAction.SetTarget(m_currentAction.userID);
                                break;
                            case TargetType.AllEnemies:
                                // if the battle only has two characters in it, then just attack one of the other character's Units
                                if (GameSettings.CHARACTERS_PER_BATTLE == 2)
                                {
                                    m_currentAction.SetTarget(new BattleUnitID(1, 0));
                                    break;
                                }
                                // otherwise let the user pick an enemy
                                goto case TargetType.SingleEnemy;
                            case TargetType.SingleEnemy:
                            case TargetType.AdjacentEnemies:
                                // activate all enemy units
                                for (int c = 1; c < GameSettings.CHARACTERS_PER_BATTLE; c++)
                                {
                                    for (int u = 0; u < m_characters[c].units.Length; u++)
                                        m_battleSceneUI.battleUnitUIArray[c * GameSettings.UNITS_PER_PARTY + u].SetAsAvailableTarget();
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
                Debug.Log(GetBattleUnit(m_currentAction.userID).displayName + " will use " +
                    GetBattleUnit(m_currentAction.userID).skillSlots[m_currentAction.skillSlotIndex].skill.skillName + " on Character " +
                    m_currentAction.targetID.character + "'s party at position " + m_currentAction.targetID.battleUnit + ".");
                m_turnActions.Add(m_currentAction);
                m_currentAction = null;

                // if all the party's actions are completed, return true
                if (m_turnActions.Count == m_characters[0].units.Length)
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
                int userUnit = action.userID.battleUnit;
                int targetCharacter = action.targetID.character;
                int targetUnit = action.targetID.battleUnit;
                bool moreThanOneUnit = GameSettings.UNITS_PER_PARTY > 1;
                bool moreThanTwoUnits = GameSettings.UNITS_PER_PARTY > 2;

                // position: 0 = left, MAX = right, else is in middle somewhere
                int userPosition = (userCharacter * GameSettings.UNITS_PER_PARTY + targetUnit) % GameSettings.UNITS_PER_PARTY;
                int targetPosition = (targetCharacter * GameSettings.UNITS_PER_PARTY + targetUnit) % GameSettings.UNITS_PER_PARTY;

                // switch uses this to hold a value
                int buffer;
                switch (user.skillSlots[action.skillSlotIndex].skill.targets)
                {
                    case TargetType.SingleEnemy:
                    case TargetType.SingleParty:
                        targets = new BattleUnit[1];
                        targets[0] = GetBattleUnit(action.targetID);
                        Debug.Log(user.displayName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + 
                            " on " + GetBattleUnit(action.targetID).displayName + "!");
                        break;
                    case TargetType.Self:
                        targets = new BattleUnit[1];
                        targets[0] = user;
                        Debug.Log(user.displayName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on itself!");
                        break;
                    case TargetType.AdjacentEnemies:
                        if (moreThanOneUnit)
                        {
                            if (targetPosition == 0 || targetPosition == GameSettings.UNITS_PER_PARTY - 1)
                            {
                                targets = new BattleUnit[2];
                                targets[0] = GetBattleUnit(action.targetID);
                                targets[1] = GetBattleUnit(targetCharacter, targetUnit + (targetPosition == 0 ? 1 : -1));
                                Debug.Log(user.displayName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on " +
                                    GetBattleUnit(action.targetID).displayName + " and " + GetBattleUnit(targetCharacter, targetUnit + 1).displayName + "!");
                            }
                            else if (moreThanTwoUnits)
                            {
                                targets = new BattleUnit[3];
                                targets[0] = GetBattleUnit(targetCharacter, targetUnit - 1);
                                targets[1] = GetBattleUnit(action.targetID);
                                targets[2] = GetBattleUnit(targetCharacter, targetUnit + 1);
                                Debug.Log(user.displayName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on " +
                                    GetBattleUnit(targetCharacter, targetUnit - 1).displayName + ", " + GetBattleUnit(action.targetID).displayName + " and  " +
                                    GetBattleUnit(targetCharacter, targetUnit + 1).displayName + "!");
                            }
                            break;
                        }
                        goto case TargetType.SingleParty;
                    case TargetType.AllEnemies:
                        targets = new BattleUnit[GameSettings.UNITS_PER_PARTY];
                        for (int u = 0; u < GameSettings.UNITS_PER_PARTY; u++)
                        {
                            targets[u] = GetBattleUnit(targetCharacter, u);
                        }
                        Debug.Log(user.displayName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on the entire enemy party!");
                        break;
                    case TargetType.WholePartyButSelf:
                        targets = new BattleUnit[GameSettings.UNITS_PER_PARTY - 1];
                        buffer = 0;
                        for (int u = 0; u < GameSettings.UNITS_PER_PARTY; u++)
                        {
                            if (GetBattleUnit(userCharacter, u).battleID != action.userID)
                                targets[u - buffer] = GetBattleUnit(userCharacter, u);
                            else
                                buffer = -1;
                        }
                        Debug.Log(user.displayName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on its party members!");
                        break;
                    case TargetType.WholeParty:
                        targets = new BattleUnit[GameSettings.UNITS_PER_PARTY];
                        for (int u = 0; u < GameSettings.UNITS_PER_PARTY; u++)
                        {
                            targets[u] = GetBattleUnit(userCharacter, u);
                        }
                        Debug.Log(user.displayName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on its whole party!");
                        break;
                    case TargetType.EveryoneButSelf:
                        targets = new BattleUnit[GameSettings.CHARACTERS_PER_BATTLE * GameSettings.UNITS_PER_PARTY - 1];
                        buffer = 0;
                        for (int c = 0; c < GameSettings.CHARACTERS_PER_BATTLE; c++)
                        {
                            for (int u = 0; u < GameSettings.UNITS_PER_PARTY; u++)
                            {
                                if (GetBattleUnit(c, u).battleID != action.userID)
                                    targets[c * GameSettings.UNITS_PER_PARTY + u + buffer] = GetBattleUnit(c, u);
                                else
                                    buffer = -1;
                            }
                        }
                        Debug.Log(user.displayName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on everyone else!");
                        break;
                    case TargetType.Everyone:
                        targets = new BattleUnit[GameSettings.CHARACTERS_PER_BATTLE * GameSettings.UNITS_PER_PARTY];
                        for (int c = 0; c < GameSettings.CHARACTERS_PER_BATTLE; c++)
                        {
                            for (int u = 0; u < GameSettings.UNITS_PER_PARTY; u++)
                            {
                                targets[c * GameSettings.CHARACTERS_PER_BATTLE + u] = GetBattleUnit(c, u);
                            }
                        }
                        Debug.Log(user.displayName + " uses " + user.skillSlots[action.skillSlotIndex].skill.skillName + " on everyone!");
                        break;
                }
            }
        }
    }
}
