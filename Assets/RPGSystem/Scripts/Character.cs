using System.Linq;
using UnityEngine;

namespace RPGSystem
{
    public abstract class Character : ScriptableObject
    {
        // personal character details
        [SerializeField] protected string m_characterName;
        
        // array of characters units
        [SerializeField] protected Unit[] m_units = new Unit[GameSettings.UNITS_PER_PARTY];

        // public accessors
        public string characterName
        {
            get { return m_characterName; }
        }
        public Unit[] units
        {
            get { return m_units; }
        }
        
        /// <summary>
        /// Cut any null entries from the Character's Units.
        /// </summary>
        public virtual void InitialiseForBattle()
        {
            m_units = m_units.Where(unit => unit != null).ToArray();
        }

        public virtual void ResetCharacter()
        {
            m_units = null;
            m_characterName = string.Empty;
        }
    }
}
