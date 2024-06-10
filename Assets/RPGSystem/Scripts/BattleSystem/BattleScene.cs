using System.Collections.Generic;
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
            End,
            WaitForExit
        }

        /// <summary>
        /// The object that stores and displays the the battle Actions
        /// </summary>
        [SerializeField] protected BattleTextLog m_textLogPrefab;

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
        public int totalUnits
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
                    if (!BattleShouldEnd())
                        m_currentPhase = BattlePhase.TurnStart;
                    else
                        m_currentPhase = BattlePhase.End;
                    break;
                case BattlePhase.End:
                    OnBattleEnd();
                    m_currentPhase = BattlePhase.WaitForExit;
                    break;
            }
        }

        /// <summary>
        /// Runs when the battle starts.
        /// </summary>
        protected virtual void OnBattleStart()
        {
            // Set turn number to 0
            m_turnNumber = 0;
            
            // only handles two characters for now
            if (m_characters.Length < 2)
            {
                Debug.LogError("Battle needs at least 2 Characters to run!");
            }

            // create the battle log canvas
            Instantiate(m_textLogPrefab, transform);

            // First line :)
            BattleTextLog.Instance.AddLine("Battle started between " + m_characters[0].characterName + " and " + m_characters[1].characterName + "!");

            // get Characters and BattleUnits ready for battle
            foreach (Character character in m_characters)
            {
                character.InitialiseForBattle();
                foreach (Unit unit in character.units)
                {
                    unit.InitialiseForBattle();
                }
            }
            InitialiseBattleUnits();
        }

        protected abstract void InitialiseBattleUnits();

        /// <summary>
        /// Runs when the turn starts.
        /// </summary>
        protected virtual void OnTurnStart()
        {
            m_turnNumber++;
            BattleTextLog.Instance.AddLine("Turn " + (m_turnNumber).ToString());
            
            // check OnTurnStart Status Effects
            for (int c = 0; c < m_characters.Length; c++)
            {
                for (int u = 0; u < m_characters[c].units.Length; u++)
                {
                    BattleUnit battleUnit = GetBattleUnit(c, u);
                    foreach (StatusSlot slot in battleUnit.statusSlots)
                    {
                        if (slot.status.onTurnStart.Length > 0)
                        {
                            BattleTextLog.Instance.AddLine(battleUnit.displayName + "'s " + slot.status.statusName + " activates!");
                            slot.OnTurnStart(battleUnit);
                            if (battleUnit.currentHP == 0)
                                UnitDefeated(slot.status.user, battleUnit, slot.status.statusName);
                        }
                    }

                    // and tick down skill slot timers
                    foreach (SkillSlot slot in battleUnit.skillSlots)
                        slot.ChangeTurnTimer(-1);
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
                    // check for on turn end status effects
                    BattleUnit battleUnit = GetBattleUnit(c, u);
                    List<StatusSlot> slotsToRemove = new List<StatusSlot>();
                    foreach (StatusSlot slot in battleUnit.statusSlots)
                    {
                        // status on turn end effects
                        if (slot.status.onTurnEnd.Length > 0)
                        {
                            BattleTextLog.Instance.AddLine(battleUnit.displayName + "'s " + slot.status.statusName + " activates!");
                            slot.OnTurnEnd(battleUnit);
                            if (battleUnit.currentHP == 0)
                                UnitDefeated(slot.status.user, battleUnit, slot.status.statusName);
                        }

                        // lower timer of status
                        slot.ChangeTurnTimer(-1);
                    }

                    // check if any statuses should be cleared
                    battleUnit.CheckStatusTimers();
                }
            }
        }

        /// <summary>
        /// Returns if the Battle should be over.
        /// </summary>
        /// <returns></returns>
        protected abstract bool BattleShouldEnd();

        /// <summary>
        /// Runs when the battle ends.
        /// </summary>
        protected virtual void OnBattleEnd()
        {
            Character lastCharacter = GetLastCharacter();
            if (lastCharacter != null)
            {
                BattleTextLog.Instance.AddLine(lastCharacter.characterName + " wins the battle!");
            }
            else
            {
                BattleTextLog.Instance.AddLine("It's a draw!");
            }
        }

        protected abstract Character GetLastCharacter();

        /// <summary>
        /// Override this method to determine how your BattleScene will accept actions.
        /// Return true when the turn is ready to be processed.
        /// </summary>
        /// <returns>If battle should progress to next phase.</returns>
        protected abstract bool WaitForActions();

        public void AddAction(Action action)
        {
            m_turnActions.Add(action);
        }

        public void RemoveAction(Action action)
        {
            m_turnActions.Remove(action);
        }

        protected abstract List<Action> OrderTurnActions();

        protected virtual void ProcessActions()
        {
            // sort action list by unit speed stats
            List<Action> orderedTurnActions = OrderTurnActions();
            
            foreach (Action action in orderedTurnActions)
            {
                BattleUnit user = GetBattleUnit(action.userID);

                if (user.currentHP <= 0)
                    continue;

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
                                BattleTextLog.Instance.AddLine(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on itself!");
                                break;
                            case TargetType.AdjacentEnemies:
                                if (m_characters[attackAction.targetID.character].units.Length > 1)
                                {
                                    if (attackAction.targetID.battleUnit == 0 || attackAction.targetID.battleUnit == GameSettings.UNITS_PER_PARTY - 1)
                                    {
                                        targets = new BattleUnit[2];
                                        targets[0] = GetBattleUnit(attackAction.targetID);
                                        targets[1] = GetBattleUnit(targetCharacter, targetUnit + (attackAction.targetID.battleUnit == 0 ? 1 : -1));
                                        BattleTextLog.Instance.AddLine(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on " +
                                            GetBattleUnit(attackAction.targetID).displayName + " and " + GetBattleUnit(targetCharacter, targetUnit + 1).displayName + "!");
                                    }
                                    else
                                    {
                                        targets = new BattleUnit[3];
                                        targets[0] = GetBattleUnit(targetCharacter, targetUnit - 1);
                                        targets[1] = GetBattleUnit(attackAction.targetID);
                                        targets[2] = GetBattleUnit(targetCharacter, targetUnit + 1);
                                        BattleTextLog.Instance.AddLine(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on " +
                                            GetBattleUnit(targetCharacter, targetUnit - 1).displayName + ", " + GetBattleUnit(attackAction.targetID).displayName + " and  " +
                                            GetBattleUnit(targetCharacter, targetUnit + 1).displayName + "!");
                                    }
                                    break;
                                }
                                else
                                    goto default;
                            case TargetType.AllEnemies:
                                targets = new BattleUnit[totalUnits - playersNumberOfUnits];
                                for (int c = 1; c < m_characters.Length; c++)
                                {
                                    for (int u = 0; u < playersNumberOfUnits; u++)
                                    {
                                        targets[u] = GetBattleUnit(targetCharacter, u);
                                    }
                                }
                                BattleTextLog.Instance.AddLine(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on all enemies!");
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
                                BattleTextLog.Instance.AddLine(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on its party members!");
                                break;
                            case TargetType.WholeParty:
                                targets = new BattleUnit[playersNumberOfUnits];
                                for (int u = 0; u < playersNumberOfUnits; u++)
                                {
                                    targets[u] = GetBattleUnit(userCharacter, u);
                                }
                                BattleTextLog.Instance.AddLine(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on its whole party!");
                                break;
                            case TargetType.EveryoneButSelf:
                                targets = new BattleUnit[totalUnits - 1];
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
                                BattleTextLog.Instance.AddLine(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on everyone else!");
                                break;
                            case TargetType.Everyone:
                                targets = new BattleUnit[totalUnits];
                                for (int c = 0; c < m_characters.Length; c++)
                                {
                                    for (int u = 0; u < m_characters[c].units.Length; u++)
                                    {
                                        targets[c * GameSettings.UNITS_PER_PARTY + u] = GetBattleUnit(c, u);
                                    }
                                }
                                BattleTextLog.Instance.AddLine(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName + " on everyone!");
                                break;
                            default:
                                targets[0] = GetBattleUnit(attackAction.targetID);
                                BattleTextLog.Instance.AddLine(user.displayName + " uses " + user.skillSlots[attackAction.skillSlotIndex].skill.skillName +
                                    " on " + GetBattleUnit(attackAction.targetID).displayName + "!");
                                break;
                        }

                        // do the effects
                        foreach (BattleUnit target in targets)
                        {
                            if (target.currentHP > 0)
                            {
                                foreach (Effect effect in user.skillSlots[attackAction.skillSlotIndex].skill.effects)
                                {  
                                    effect.DoEffect(user, target);
                                    if (target.currentHP <= 0)
                                        UnitDefeated(user, target);
                                }
                            }
                            else
                                BattleTextLog.Instance.AddLine("But there was no target!");
                        }

                        // set the cooldown on the used skill
                        user.skillSlots[attackAction.skillSlotIndex].ChangeTurnTimer(user.skillSlots[attackAction.skillSlotIndex].skill.turnTimer);

                        break;

                    case SkipAction skipAction:
                        BattleTextLog.Instance.AddLine(GetBattleUnit(skipAction.userID).displayName + " does nothing!");
                        break;

                }
            }
            m_turnActions.Clear();
        }

        protected virtual void UnitDefeated(BattleUnit user, BattleUnit target, string causeOfDefeat = "")
        {
            if (causeOfDefeat == string.Empty)
                BattleTextLog.Instance.AddLine(user.displayName + " defeated " + target.displayName + "!");
            else
                BattleTextLog.Instance.AddLine(target.displayName + " was deafeated by " + causeOfDefeat);

            user.unit.GainExp(target.unit.GetExpWorth());
        }
    }
}
