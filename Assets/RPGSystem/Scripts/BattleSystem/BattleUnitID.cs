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
}