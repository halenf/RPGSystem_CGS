using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    [CreateAssetMenu(fileName = "Settings", menuName = "RPGSystem/Game Settings", order = 0)]
    public class GameSettingsAsset : ScriptableObject
    {
        [SerializeField] protected int m_monstersPerParty;
        [SerializeField] protected int m_maxMonsterLevel;
        [SerializeField] protected int m_charactersPerBattle;
        [SerializeField] protected int m_maxSkillsPerCharacter;
        [SerializeField] protected int m_maxSkillsPerMonster;

        public int monstersPerParty
        {
            get { return m_monstersPerParty; }
        }
        public int maxMonsterLevel
        {
            get { return m_maxMonsterLevel; }
        }
        public int charactersPerBattle
        {
            get { return m_charactersPerBattle; }
        }
        public int maxSkillsPerCharacter
        {
            get { return m_maxSkillsPerCharacter; }
        }
        public int maxSkillsPerMonster
        {
            get { return m_maxSkillsPerMonster; }
        }
    }
}
