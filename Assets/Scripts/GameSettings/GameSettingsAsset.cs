using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    [CreateAssetMenu(fileName = "Settings", menuName = "RPGSystem/Game Settings", order = 0)]
    public class GameSettingsAsset : ScriptableObject
    {
        [SerializeField] protected int m_unitsPerParty;
        [SerializeField] protected int m_maxUnitLevel;
        [SerializeField] protected int m_charactersPerBattle;
        [SerializeField] protected int m_maxSkillsPerCharacter;
        [SerializeField] protected int m_maxSkillsPerUnit;

        public int unitsPerParty
        {
            get { return m_unitsPerParty; }
        }
        public int maxUnitLevel
        {
            get { return m_maxUnitLevel; }
        }
        public int charactersPerBattle
        {
            get { return m_charactersPerBattle; }
        }
        public int maxSkillsPerCharacter
        {
            get { return m_maxSkillsPerCharacter; }
        }
        public int maxSkillsPerUnit
        {
            get { return m_maxSkillsPerUnit; }
        }
    }
}
