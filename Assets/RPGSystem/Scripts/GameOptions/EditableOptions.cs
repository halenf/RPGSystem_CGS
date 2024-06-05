using System;
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
        public static string[] STAT_NAMES = { "Max HP", "Attack", "Defence", "Speed" };

        // custom settings go here
        public static int MAX_SKILLS_PER_CHARACTER = 3;

        public static void LoadSettingsFromSO(GameSettingsAsset set)
        {
            UNITS_PER_PARTY = set.unitsPerParty;
            MAX_UNIT_LEVEL = set.maxUnitLevel;
            CHARACTERS_PER_BATTLE = set.charactersPerBattle;
            MAX_SKILLS_PER_UNIT = set.maxSkillsPerUnit;
            MAX_STAT_MODIFIER = set.maxStatModifier;
            STAT_NAMES = set.statNames;

            // set custom settings here
            MAX_SKILLS_PER_CHARACTER = set.maxSkillsPerCharacter;

            Debug.Log("Successfully loaded " + set.name + " into GameSettings.");
        }
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
        Lifesteal = 16, // Implemented
        Stun = 32,
        Invulnerable = 64
    }

    // The names of the Base Stats in the game
    [Serializable]
    public enum BaseStatName
    {
        Health = 0,
        Strength = 1,
        Fortitude = 2,
        Agility = 3
    }
}
