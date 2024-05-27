using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    public static class GameSettings
    {       
        // core settings here
        public static int UNITS_PER_PARTY = 3;
        public static int MAX_UNIT_LEVEL = 100;
        public static int CHARACTERS_PER_BATTLE = 2;
        public static int MAX_SKILLS_PER_UNIT = 3;
        public static float MAX_STAT_MODIFIER = 2;

        // custom settings go here
        public static int MAX_SKILLS_PER_CHARACTER = 3;

        public static void LoadSettingsFromSO(GameSettingsAsset set)
        {
            UNITS_PER_PARTY = set.unitsPerParty;
            MAX_UNIT_LEVEL = set.maxUnitLevel;
            CHARACTERS_PER_BATTLE = set.charactersPerBattle;
            MAX_SKILLS_PER_UNIT = set.maxSkillsPerUnit;
            MAX_STAT_MODIFIER = 

            // set custom settings here
            MAX_SKILLS_PER_CHARACTER = set.maxSkillsPerCharacter;
        }
    }

    [CreateAssetMenu(fileName = "Settings", menuName = "RPGSystem/Game Settings", order = 0)]
    public class GameSettingsAsset : ScriptableObject
    {
        [Header("Core Settings")]
        [SerializeField] protected int m_unitsPerParty;
        [SerializeField] protected int m_maxUnitLevel;
        [SerializeField] protected int m_charactersPerBattle;
        [SerializeField] protected int m_maxSkillsPerUnit;
        [SerializeField] protected float m_maxStatModifier;

        [Header("Custom Settings")]
        [SerializeField] protected int m_maxSkillsPerCharacter;

        // core settings accessors
        public int unitsPerParty { get { return m_unitsPerParty; } }
        public int maxUnitLevel { get { return m_maxUnitLevel; } }
        public int charactersPerBattle { get { return m_charactersPerBattle; } }
        public int maxSkillsPerUnit { get { return m_maxSkillsPerUnit; } }
        public float maxStatModifier { get { return m_maxStatModifier; } }

        // custom settings accessors here
        public int maxSkillsPerCharacter{ get { return m_maxSkillsPerCharacter; } }
    }

    // Special Effects that have triggers required to be placed somehwere else in the codespace
    [Serializable]
    [Flags]
    public enum TriggeredEffect
    {
        None = 0,
        FailNextSkillEffect = 1,
        FailStatusClearEffects = 2, // Implemented
        DamageOnSkillUse = 4,
        DebuffImmunity = 8, // Implemented
        Lifesteal = 16 // Implemented
    }

    // The names of the Base Stats in the game
    public enum UnitBaseStatNames
    {
        Health = 0,
        Strength = 1,
        Fortitude = 2,
        Agility = 3
    }

    // class for UnitData to hold base stat values
    [Serializable]
    public struct BaseStats
    {
        [SerializeField] private int m_health, m_strength, m_fortitude, m_agility;

        public int health { get { return m_health; } }
        public int strength { get { return m_strength; } }
        public int fortitude { get { return m_fortitude; } }
        public int agility { get { return m_agility; } }
    }
}
