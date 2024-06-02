using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RPGSystem
{
    using TargetType = Skill.TargetType;

    public abstract class BattleScene : MonoBehaviour
    {
        // The type of battle
        protected enum BattleType
        {
            Wild, // Against unit(s) without a party
            Party // Against another party
        }
        // battle phase enum
        protected enum BattlePhase
        {
            Start,
            TurnStart,
            ChooseActions,
            Action,
            TurnEnd,
            End
        }

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
        /// The type of battle the player is participating in.
        /// </summary>
        protected BattleType battleType;

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
        /// Gets the specified BattleUnit from the specified Character.
        /// </summary>
        /// <param name="character">Index representing the selected Character.</param>
        /// <param name="unit">Index representing the selected BattleUnit.</param>
        /// <returns></returns>
        public BattleUnit GetBattleUnit(int character, int unit)
        {
            return GetBattleUnit(new BattleUnitID(character, unit));
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

        /// <summary>
        /// Returns the total number of Units participating in the battle.
        /// </summary>
        public int TotalUnits
        {
            get
            {
                int value = 0;
                foreach (Character c in m_characters)
                    value += c.units.Length;
                return value;
            } 
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
                    OnTurnStart();
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
                    OnTurnEnd();
                    m_currentPhase = BattlePhase.TurnStart;
                    break;
                case BattlePhase.End:
                    OnBattleEnd();
                    break;
            }
        }

        /// <summary>
        /// Runs when the battle starts. Override to add your own method of initialising m_battleUnits.
        /// </summary>
        protected virtual void OnBattleStart()
        {
            // only handles two characters for now
            if (m_characters.Length < 2)
            {
                Debug.LogError("Battle needs at least 2 Characters to run!");
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
            }

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
        }

        /// <summary>
        /// Runs when the turn starts.
        /// </summary>
        protected virtual void OnTurnStart()
        {
            // check OnTurnStart Status Effects
            for (int c = 0; c < m_characters.Length; c++)
            {
                for (int u = 0; u < m_characters[c].units.Length; u++)
                {
                    BattleUnit battleUnit = GetBattleUnit(c, u);
                    if (battleUnit.statusSlots.Count > 0)
                    {
                        foreach (StatusSlot slot in battleUnit.statusSlots)
                        {
                            Debug.Log(battleUnit.displayName + "'s " + slot.status.statusName);
                            foreach (Effect effect in slot.status.onTurnStart)
                                effect.DoEffect(slot.status.user, new BattleUnit[] { battleUnit });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Runs when the turn ends.
        /// </summary>
        protected virtual void OnTurnEnd()
        {
            // check OnTurnEnd Status Effects
            for (int c = 0; c < m_characters.Length; c++)
            {
                for (int u = 0; u < m_characters[c].units.Length; u++)
                {
                    BattleUnit battleUnit = GetBattleUnit(c, u);
                    if (battleUnit.statusSlots.Count > 0)
                    {
                        foreach (StatusSlot slot in battleUnit.statusSlots)
                        {
                            Debug.Log(battleUnit.displayName + "'s " + slot.status.statusName);
                            foreach (Effect effect in slot.status.onTurnEnd)
                                effect.DoEffect(slot.status.user, new BattleUnit[] { battleUnit });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Runs when the battle ends.
        /// </summary>
        protected virtual void OnBattleEnd()
        {

        }

        /// <summary>
        /// Override this method to determine how your BattleScene will accept actions.
        /// Return true when the turn is ready to be processed.
        /// </summary>
        /// <returns>If battle is ready to progress.</returns>
        protected abstract bool WaitForActions();

        public void AddAction(Action action)
        {
            m_turnActions.Add(action);
        }

        public void RemoveAction(Action action)
        {
            m_turnActions.Remove(action);
        }

        protected virtual void ProcessActions()
        {
            // sort action list by unit speed stats
            List<Action> orderedTurnActions = m_turnActions.OrderBy(action => action.order).ToList();
            
            foreach (Action action in orderedTurnActions)
            {
                BattleUnit user = GetBattleUnit(action.userID);

                switch (action)
                {
                    case AttackAction attackAction:
                        // has to be initialised with a length
                        BattleUnit[] targets = new BattleUnit[1];

                        // quick reference
                        int userCharacter = attackAction.userID.character;
                        int userUnit = attackAction.userID.battleUnit;
                        int targetCharacter = attackAction.targetID.character;
                        int targetUnit = attackAction.targetID.battleUnit;
                        int playersNumberOfUnits = m_characters[0].units.Length;

                        // switch uses this to hold a value
                        int buffer;
                        switch (user.skillSlots[attackAction.skillSlotIndex].skill.targets)
                        {
                            case TargetType.Self:
                                targets = new BattleUnit[1];
                                targets[0] = user;
                                Debug.Log(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on itself!");
                                break;
                            case TargetType.AdjacentEnemies:
                                if (m_characters[attackAction.targetID.character].units.Length > 1)
                                {
                                    if (attackAction.targetID.battleUnit == 0 || attackAction.targetID.battleUnit == GameSettings.UNITS_PER_PARTY - 1)
                                    {
                                        targets = new BattleUnit[2];
                                        targets[0] = GetBattleUnit(attackAction.targetID);
                                        targets[1] = GetBattleUnit(targetCharacter, targetUnit + (attackAction.targetID.battleUnit == 0 ? 1 : -1));
                                        Debug.Log(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on " +
                                            GetBattleUnit(attackAction.targetID).displayName + " and " + GetBattleUnit(targetCharacter, targetUnit + 1).displayName + "!");
                                    }
                                    else
                                    {
                                        targets = new BattleUnit[3];
                                        targets[0] = GetBattleUnit(targetCharacter, targetUnit - 1);
                                        targets[1] = GetBattleUnit(attackAction.targetID);
                                        targets[2] = GetBattleUnit(targetCharacter, targetUnit + 1);
                                        Debug.Log(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on " +
                                            GetBattleUnit(targetCharacter, targetUnit - 1).displayName + ", " + GetBattleUnit(attackAction.targetID).displayName + " and  " +
                                            GetBattleUnit(targetCharacter, targetUnit + 1).displayName + "!");
                                    }
                                    break;
                                }
                                else
                                    goto default;
                            case TargetType.AllEnemies:
                                targets = new BattleUnit[TotalUnits - playersNumberOfUnits];
                                for (int c = 1; c < m_characters.Length; c++)
                                {
                                    for (int u = 0; u < playersNumberOfUnits; u++)
                                    {
                                        targets[u] = GetBattleUnit(targetCharacter, u);
                                    }
                                }
                                Debug.Log(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on the entire enemy party!");
                                break;
                            case TargetType.WholePartyButSelf:
                                targets = new BattleUnit[playersNumberOfUnits - 1];
                                buffer = 0;
                                for (int u = 0; u < playersNumberOfUnits; u++)
                                {
                                    if (GetBattleUnit(userCharacter, u).battleID != attackAction.userID)
                                        targets[u - buffer] = GetBattleUnit(userCharacter, u);
                                    else
                                        buffer = -1;
                                }
                                Debug.Log(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on its party members!");
                                break;
                            case TargetType.WholeParty:
                                targets = new BattleUnit[playersNumberOfUnits];
                                for (int u = 0; u < playersNumberOfUnits; u++)
                                {
                                    targets[u] = GetBattleUnit(userCharacter, u);
                                }
                                Debug.Log(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on its whole party!");
                                break;
                            case TargetType.EveryoneButSelf:
                                targets = new BattleUnit[TotalUnits - 1];
                                buffer = 0;
                                for (int c = 0; c < m_characters.Length; c++)
                                {
                                    for (int u = 0; u < m_characters[c].units.Length; u++)
                                    {
                                        if (GetBattleUnit(c, u).battleID != attackAction.userID)
                                            targets[c * GameSettings.UNITS_PER_PARTY + u + buffer] = GetBattleUnit(c, u);
                                        else
                                            buffer = -1;
                                    }
                                }
                                Debug.Log(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on everyone else!");
                                break;
                            case TargetType.Everyone:
                                targets = new BattleUnit[TotalUnits];
                                for (int c = 0; c < m_characters.Length; c++)
                                {
                                    for (int u = 0; u < m_characters[c].units.Length; u++)
                                    {
                                        targets[c * GameSettings.UNITS_PER_PARTY + u] = GetBattleUnit(c, u);
                                    }
                                }
                                Debug.Log(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on everyone!");
                                break;
                            default:
                                targets[0] = GetBattleUnit(attackAction.targetID);
                                Debug.Log(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName +
                                    " on " + GetBattleUnit(attackAction.targetID).displayName + "!");
                                break;
                        }

                        // do the effects
                        foreach (Effect effect in user.skillSlots[attackAction.skillSlotIndex].skill.effects)
                            effect.DoEffect(user, targets);
                        break;
                }
            }
        }
    }
}
