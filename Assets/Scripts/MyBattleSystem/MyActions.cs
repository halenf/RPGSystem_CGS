using RPGSystem;

public class SkipAction : Action
{
    public SkipAction(BattleUnitID user)
    {
        m_userID = user;
    }
    public SkipAction(int character, int user)
    {
        m_userID = new BattleUnitID(character, user);
    }
}

public class DefeatedAction : Action
{
    public DefeatedAction(BattleUnitID user)
    {
        m_userID = user;
    }
    public DefeatedAction(int character, int user)
    {
        m_userID = new BattleUnitID(character, user);
    }
}

public class AttackAction : Action
{
    public AttackAction() { isDirty = true; }
    public AttackAction(BattleUnitID user, int skillIndex, BattleUnitID target)
    {
        m_userID = user;
        m_skillSlotIndex = skillIndex;
        m_targetID = target;
        isDirty = true;
    }

    protected int m_skillSlotIndex;
    protected BattleUnitID m_targetID;

    protected int m_buildState;
    public bool isDirty;

    public BattleUnitID targetID { get { return m_targetID; } }
    public int skillSlotIndex { get { return m_skillSlotIndex; } }
    public int buildState { get { return m_buildState; } }

    public override void SetUser(BattleUnitID userID)
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
