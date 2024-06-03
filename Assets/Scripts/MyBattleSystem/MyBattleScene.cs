using RPGSystem;
using UnityEngine;
using static RPGSystem.Skill;

public class MyBattleScene : BattleScene
{
    /// <summary>
    /// The current action being built.
    /// </summary>
    private Action m_currentAction;

    /// <summary>
    /// Prefab for the MyBattleSceneUI.
    /// </summary>
    [SerializeField] private MyBattleSceneUI m_battleSceneUIPrefab;
    private MyBattleSceneUI m_battleSceneUI;

    protected override void OnBattleStart()
    {
        base.OnBattleStart();

        m_battleUnits = new MyBattleUnit[m_characters.Length * GameSettings.UNITS_PER_PARTY];
        for (int c = 0; c < m_characters.Length; c++)
        {
            for (int u = 0; u < m_characters[c].units.Length; u++)
            {
                int index = c * GameSettings.UNITS_PER_PARTY + u;
                m_battleUnits[index] = new MyBattleUnit(m_characters[c].units[u] as MyUnit, new BattleUnitID(c, u));
                m_battleUnits[index].ResetBattleUnit();
                Debug.Log(m_characters[c].characterName + " sends out " + GetBattleUnit(c, u).displayName + "!");
            }
        }

        // Instantiates the MyBattleSceneUI
        m_battleSceneUI = Instantiate(m_battleSceneUIPrefab);
        m_battleSceneUI.Initialise(this);
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
        m_currentAction ??= new AttackAction();

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
                            Debug.Log("Choose one of your Units.");
                            // activate the parties units to be selected as users
                            for (int u = 0; u < m_characters[0].units.Length; u++)
                            {
                                // if the turn actions already contains that unit's action, then don't make it available as a user
                                if (m_turnActions.Find(action => (action as AttackAction).userID == new BattleUnitID(0, u)) == null)
                                    m_battleSceneUI.battleUnitUIArray[u].SetAsAvailableUser();
                            }
                            break;
                        // if has a user, but no skill or targets
                        case 1:
                            Debug.Log("Choose a skill for " + GetBattleUnit(attackAction.userID).displayName + " to use.");
                            // activate the skill slots for the selected unit
                            for (int s = 0; s < GetBattleUnit(0, attackAction.userID.battleUnit).skillSlots.Count; s++)
                            {
                                int skillIndex = attackAction.userID.battleUnit * GameSettings.MAX_SKILLS_PER_UNIT + s;
                                m_battleSceneUI.skillSlotUIArray[skillIndex].SetAsAvailableSkill(true);
                            }
                            break;
                        // if has user and skill, but no targets
                        case 2:
                            Debug.Log("Choose the target for " + GetBattleUnit(attackAction.userID).skillSlots[attackAction.skillSlotIndex].skill.skillName + ".");
                            // activate the units based on the TargetType of the skill
                            switch (GetBattleUnit(attackAction.userID).skillSlots[attackAction.skillSlotIndex].skill.targets)
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
                                    // activate all enemy units
                                    for (int c = 1; c < m_characters.Length; c++)
                                    {
                                        for (int u = 0; u < m_characters[c].units.Length; u++)
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
                        Debug.Log(GetBattleUnit(attackAction.userID).displayName + " will use " +
                            GetBattleUnit(attackAction.userID).skillSlots[attackAction.skillSlotIndex].skill.skillName + " on Character " +
                            attackAction.targetID.character + "'s party at position " + attackAction.targetID.battleUnit + ".");
                        m_turnActions.Add(attackAction);
                        m_currentAction = null;
                    }
                }
                break;
        }

        // if all the party's actions are completed, return true
        if (m_turnActions.Count == m_characters[0].units.Length)
            return true;

        return false;
    }

    public void AddAttackAction(BattleUnitID userID, int skillSlotIndex, BattleUnitID targetID)
    {
        m_turnActions.Add(new AttackAction(userID, skillSlotIndex, targetID, (GetBattleUnit(userID) as MyBattleUnit).speed));
    }

    public void RemoveAttackAction(BattleUnitID userID)
    {
        m_turnActions.RemoveAll(action => (action as AttackAction).userID == userID);
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
}
