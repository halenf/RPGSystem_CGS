using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    public static class GameSettings
    {       
        public static int UNITS_PER_PARTY = 3;
        public static int MAX_UNIT_LEVEL = 100;
        public static int CHARACTERS_PER_BATTLE = 2;
        public static int MAX_SKILLS_PER_CHARACTER = 3;
        public static int MAX_SKILLS_PER_UNIT = 3;

        public static void LoadSettingsFromSO(GameSettingsAsset set)
        {
            UNITS_PER_PARTY = set.unitsPerParty;
            MAX_UNIT_LEVEL = set.maxUnitLevel;
            CHARACTERS_PER_BATTLE = set.charactersPerBattle;
            MAX_SKILLS_PER_CHARACTER = set.maxSkillsPerCharacter;
            MAX_SKILLS_PER_UNIT = set.maxSkillsPerUnit;
        }
    }
}
