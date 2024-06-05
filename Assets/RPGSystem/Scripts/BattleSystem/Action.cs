namespace RPGSystem
{
    public abstract class Action
    {
        protected BattleUnitID m_userID;
        public BattleUnitID userID { get { return m_userID; } }

        public virtual void SetUser(BattleUnitID userID)
        {
            m_userID = userID;
        }
    }
}
