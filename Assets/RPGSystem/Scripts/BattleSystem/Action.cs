namespace RPGSystem
{
    public abstract class Action
    {
        protected BattleUnitID m_userID;
        protected int m_order;
        public int order { get { return m_order; } }
        public BattleUnitID userID { get { return m_userID; } }

        public virtual void SetUser(BattleUnitID userID)
        {
            m_userID = userID;
        }
        public void SetOrder(int order)
        {
            m_order = order;
        }
    }
}
