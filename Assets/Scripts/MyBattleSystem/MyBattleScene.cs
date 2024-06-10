using RPGSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RPGSystem.Skill;

public class MyBattleScene : BattleScene
{
    /// <summary>
    /// The current action being built.
    /// </summary>
    private Action m_currentAction;

    /// <summary>
    /// If a battle is currently in progress
    /// </summary>
    private bool m_isPlaying;
    public bool isPlaying { get { return m_isPlaying; } }

    /// <summary>
    /// Prefab for the MyBattleSceneUI.
    /// </summary>
    [Header("Custom Variables")]
    [SerializeField] private MyBattleSceneUI m_battleSceneUIPrefab;
    private MyBattleSceneUI m_battleSceneUI;

    [SerializeField] private GameObject m_endBattleButton;

    public void Initialise(Character[] characters)
    {
        m_characters = characters;
        m_isPlaying = true;
    }

    new private void Start()
    {
        base.Start();
        m_endBattleButton.SetActive(false);
    }

    public void SetCharacters(MyCharacter[] characters)
    {
        m_characters = characters;
    }

    protected override void OnBattleStart()
    {
        base.OnBattleStart();

        // Instantiates the MyBattleSceneUI
        m_battleSceneUI = Instantiate(m_battleSceneUIPrefab);
        m_battleSceneUI.Initialise(this);
    }

    protected override void InitialiseBattleUnits()
    {
        m_battleUnits = new MyBattleUnit[m_characters.Length * GameSettings.UNITS_PER_PARTY];
        for (int c = 0; c < m_characters.Length; c++)
        {
            for (int u = 0; u < m_characters[c].units.Length; u++)
            {
                int index = c * GameSettings.UNITS_PER_PARTY + u;
                m_battleUnits[index] = new MyBattleUnit(m_characters[c].units[u] as MyUnit, new BattleUnitID(c, u));
                m_battleUnits[index].ResetBattleUnit();
                BattleTextLog.Instance.AddLine(m_characters[c].characterName + " sends out " + GetBattleUnit(c, u).displayName + "!");
            }
        }
    }

    protected override void OnTurnStart()
    {
        base.OnTurnStart();

        for (int c = 0; c < m_characters.Length; c++)
        {
            for (int u = 0; u < m_characters[c].units.Length; u++)
            {
                BattleUnit battleUnit = GetBattleUnit(c, u);
                StatusSlot ventStatusSlot = battleUnit.statusSlots.Find(slot => slot.status.statusName == "Vent");
                if (ventStatusSlot != null)
                {
                    if (ventStatusSlot.turnTimer == 0)
                    {
                        MyStatus ventStatus = ventStatusSlot.status as MyStatus;
                        foreach (BattleUnit target in ventStatus.targets)
                            ventStatusSlot.OnClear(target);
                        battleUnit.RemoveStatusSlot(ventStatus);
                        BattleTextLog.Instance.AddLine(battleUnit.displayName + " lost Vent!");
                    }
                }
            }
        }

        // check if a unit should skip its turn
        // only run this once per turn
        for (int u = 0; u < m_characters[0].units.Length; u++)
        {
            MyBattleUnit battleUnit = GetBattleUnit(0, u) as MyBattleUnit;
            if (battleUnit.currentHP == 0)
            {
                m_turnActions.Add(new DefeatedAction(0, u));
            }
            else if (battleUnit.cantActThisTurn)
            {
                m_turnActions.Add(new SkipAction(new BattleUnitID(0, u)));
            }
        }
    }

    /// <summary>
    /// Build actions from UI object buttons.
    /// When action has user, skill id, and targets, action is complete and added to action list.
    /// When there is an action for every unit, this method returns true and the actions are done.
    /// </summary>
    /// <returns>If all units have an action.</returns>
    protected override bool WaitForActions()
    {
        // if null, create new from default constructor
        if (m_currentAction == null)
        {
            // if all units should skip their turn, dont create an attack action
            if (m_turnActions.Count != m_characters[0].units.Length)
                m_currentAction = new AttackAction();
        }

        // logic for building the current action from UI
        switch (m_currentAction)
        {
            case AttackAction attackAction:
                // if the action has just been created or edited
                if (attackAction.isDirty)
                {
                    // reset the state of all the UI
                    for (int u = 0; u < m_battleSceneUI.battleUnitUIArray.Length; u++)
                        if (m_battleSceneUI.battleUnitUIArray[u] != null)
                            m_battleSceneUI.battleUnitUIArray[u].SetAsUnavailable();
                    for (int s = 0; s < m_battleSceneUI.skillSlotUIArray.Length; s++)
                        if (m_battleSceneUI.skillSlotUIArray[s] != null)
                            m_battleSceneUI.skillSlotUIArray[s].SetAsAvailableSkill(false);

                    // activate the relevant UI objects
                    switch (attackAction.buildState)
                    {
                        // if has no user, skill, or targets
                        case 0:
                            BattleTextLog.Instance.AddLine("Choose one of your Units.");
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
                            BattleTextLog.Instance.AddLine("Choose a skill for " + GetBattleUnit(attackAction.userID).displayName + " to use.");
                            // activate the skill slots for the selected unit
                            for (int s = 0; s < GetBattleUnit(0, attackAction.userID.battleUnit).skillSlots.Count; s++)
                            {
                                if (GetBattleUnit(0, attackAction.userID.battleUnit).skillSlots[s].turnTimer == 0)
                                {
                                    int skillIndex = attackAction.userID.battleUnit * GameSettings.MAX_SKILLS_PER_UNIT + s;
                                    m_battleSceneUI.skillSlotUIArray[skillIndex].SetAsAvailableSkill(true);
                                }
                            }
                            break;
                        // if has user and skill, but no targets
                        case 2:
                            // activate the units based on the TargetType of the skill
                            switch (GetBattleUnit(attackAction.userID).skillSlots[attackAction.skillSlotIndex].skill.targets)
                            {
                                case TargetType.SingleParty:
                                    BattleTextLog.Instance.AddLine("Choose the target for " + GetBattleUnit(attackAction.userID).skillSlots[attackAction.skillSlotIndex].skill.skillName + ".");
                                    // activate the parties units to be selected as targets
                                    for (int c = 0; c < m_characters.Length; c++)
                                    {
                                        for (int u = 0; u < m_characters[c].units.Length; u++)
                                        {
                                            if (GetBattleUnit(c, u).currentHP > 0)
                                                m_battleSceneUI.battleUnitUIArray[u].SetAsAvailableTarget();
                                        }
                                    }
                                    break;
                                case TargetType.Self:
                                case TargetType.EveryoneButSelf:
                                case TargetType.Everyone:
                                case TargetType.WholePartyButSelf:
                                case TargetType.WholeParty:
                                    // Target Types where the Skill's target is irrelevant
                                    attackAction.SetTarget(attackAction.userID);
                                    break;
                                case TargetType.AllEnemies:
                                    // if the battle only has two characters in it, then just attack one of the other character's Units
                                    if (GameSettings.CHARACTERS_PER_BATTLE == 2)
                                    {
                                        attackAction.SetTarget(new BattleUnitID(1, 0));
                                        break;
                                    }
                                    // otherwise let the user pick an enemy
                                    goto case TargetType.SingleEnemy;
                                case TargetType.SingleEnemy:
                                case TargetType.AdjacentEnemies:
                                    BattleTextLog.Instance.AddLine("Choose the target for " + GetBattleUnit(attackAction.userID).skillSlots[attackAction.skillSlotIndex].skill.skillName + ".");
                                    // activate all enemy units
                                    for (int c = 1; c < m_characters.Length; c++)
                                    {
                                        for (int u = 0; u < m_characters[c].units.Length; u++)
                                            if (GetBattleUnit(c, u).currentHP > 0)
                                                m_battleSceneUI.battleUnitUIArray[c * GameSettings.UNITS_PER_PARTY + u].SetAsAvailableTarget();
                                    }
                                    break;
                            }

                            break;
                    }

                    // don't do this every frame
                    attackAction.isDirty = false;

                    // then check if action is complete
                    if (attackAction.buildState == 3)
                    {
                        BattleTextLog.Instance.AddLine(GetBattleUnit(attackAction.userID).displayName + " will use " +
                            GetBattleUnit(attackAction.userID).skillSlots[attackAction.skillSlotIndex].skill.skillName + " on Character " +
                            attackAction.targetID.character + "'s party at position " + attackAction.targetID.battleUnit + ".");
                        m_turnActions.Add(attackAction);
                        m_currentAction = null;
                    }
                }
                break;

            case null:
                break;
        }

        // if all the party's actions are completed, return true
        if (m_turnActions.Count == m_characters[0].units.Length)
        {
            if (m_turnNumber == 2)
                Debug.Log("Before crash");

            GetAIActions();
            return true;
        }

        return false;
    }

    public void AddUserToCurrentAttackAction(BattleUnitID userID)
    {
        m_currentAction.SetUser(userID);
    }

    public void AddSkillToCurrentAttackAction(int skillSlotIndex)
    {
        if (m_currentAction.GetType() == typeof(AttackAction))
            (m_currentAction as AttackAction).SetSkillIndex(skillSlotIndex);
    }

    public void AddTargetToCurrentAttackAction(BattleUnitID targetID)
    {
        if (m_currentAction.GetType() == typeof(AttackAction))
            (m_currentAction as AttackAction).SetTarget(targetID);
    }

    protected override List<Action> OrderTurnActions()
    {
        List<SkipAction> skip = m_turnActions.OfType<SkipAction>().ToList();
        List<AttackAction> attacks = m_turnActions.OfType<AttackAction>().ToList();

        skip = skip.OrderByDescending(skip => GetBattleUnit(skip.userID).GetStat(BaseStatName.Agility)).ToList();
        attacks = attacks.OrderByDescending(attack => (int)GetBattleUnit(attack.userID).skillSlots[attack.skillSlotIndex].skill.priority).
            ThenByDescending(attack => GetBattleUnit(attack.userID).GetStat(BaseStatName.Agility)).ToList();

        List<Action> result = skip.Concat<Action>(attacks).ToList();

        return result;
    }

    protected override Character GetLastCharacter()
    {
        for (int c = 0; c < m_characters.Length; c++)
        {
            if ((m_characters[c] as MyCharacter).hasUnitsForBattle)
               return m_characters[c];
        }
        return null;
    }

    private void GetAIActions()
    {
        for (int c = 1; c < m_characters.Length; c++)
        {
            for (int u = 0; u < m_characters[c].units.Length; u++)
            {
                MyBattleUnit battleUnit = GetBattleUnit(c, u) as MyBattleUnit;
                if (battleUnit.currentHP == 0)
                {
                    m_turnActions.Add(new DefeatedAction(c, u));
                }
                else if (battleUnit.cantActThisTurn)
                {
                    m_turnActions.Add(new SkipAction(c, u));
                }
                else
                {
                    // AI choose target character
                    int targetCharacter = Random.Range(0, m_characters.Length);
                    while (targetCharacter == c || !(m_characters[targetCharacter] as MyCharacter).hasUnitsForBattle)
                    {
                        targetCharacter++;
                        if (targetCharacter == m_characters.Length)
                            targetCharacter = 0;
                    }

                    // AI choose target unit
                    int targetUnit = Random.Range(0, m_characters[targetCharacter].units.Length);
                    while (m_characters[targetCharacter].units[targetUnit].currentHP <= 0)
                    {
                        targetUnit++;
                        if (targetUnit == m_characters[targetCharacter].units.Length)
                            targetUnit = 0;
                    }
                    
                    // ai choose skill to use
                    int skillIndex = Random.Range(0, battleUnit.skillSlots.Count);
                    while (battleUnit.skillSlots[skillIndex].turnTimer != 0)
                    {
                        skillIndex++;
                        if (skillIndex == battleUnit.skillSlots.Count)
                            skillIndex = 0;
                    }

                    // add the attack action
                    m_turnActions.Add(new AttackAction(new BattleUnitID(c, u), skillIndex, new BattleUnitID(targetCharacter, targetUnit)));
                }
            }
        }
    }

    protected override bool BattleShouldEnd()
    {
        int numOfCharactersAlive = 0;
        for (int c = 0; c < m_characters.Length; c++)
        {
            for (int u = 0; u < m_characters[c].units.Length; u++)
            {
                if (m_characters[c].units[u].currentHP > 0)
                {
                    numOfCharactersAlive++;
                    break;
                }
            }
        }

        if (numOfCharactersAlive < 2)
            return true;
        else
            return false;
    }

    protected override void UnitDefeated(BattleUnit user, BattleUnit target, string causeOfDefeat = "")
    {
        if (causeOfDefeat == string.Empty)
            BattleTextLog.Instance.AddLine(user.displayName + " defeated " + target.displayName + "!");
        else
            BattleTextLog.Instance.AddLine(target.displayName + " was deafeated by " + causeOfDefeat);

        // dont gain exp because this is just a battle tester
    }

    protected override void OnBattleEnd()
    {
        base.OnBattleEnd();
        m_endBattleButton.SetActive(true);
    }

    public void EndBattle()
    {
        Destroy(m_battleSceneUI.gameObject);
        m_isPlaying = false;
    }
}
