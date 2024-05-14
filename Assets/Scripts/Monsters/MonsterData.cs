using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    public enum MonsterBaseStats
    {
        Health = 0,
        Strength = 1,
        Fortitude = 2,
        Agility = 3
    }

    public enum MonsterLevelCurve
    {
        Slow = 220,
        Medium = 200,
        Fast = 285
    }

    [System.Serializable]
    public struct BaseStats
    {
        public int health, strength, fortitude, agility;
    }

    [System.Serializable]
    public struct LevelUpSkill
    {
        public int level;
        public Skill skill;
    }

    [CreateAssetMenu(fileName = "MonsterData", menuName = "RPGSystem_SO/Monsters/MonsterData", order = 1)]
    public class MonsterData : ScriptableObject
    {
        // monster name
        [SerializeField] private string m_monsterName;
        // monster sprite
        [SerializeField] private Sprite m_monsterSprite;
        // values that represent how the monster will grow
        [SerializeField] private BaseStats m_baseStats;
        // represents the amount of experience a monster needs to level up
        [SerializeField] private MonsterLevelCurve m_levelCurve;
        // list of skills a monster can learn as it levels up
        [SerializeField] private LevelUpSkill[] m_levelUpSkills;
        
        public string monsterName
        {
            get { return m_monsterName; }
        }
        public Sprite monsterSprite
        {
            get { return m_monsterSprite; }
        }
        public BaseStats baseStats
        {
            get
            {
                return m_baseStats;
            }
        }
        public MonsterLevelCurve levelCurve
        {
            get
            {
                return m_levelCurve;
            }
        }
        public LevelUpSkill[] levelUpSkills
        {
            get
            {
                return m_levelUpSkills;
            }
        }
    }
}
