using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    public static class GameSettings
    {
        public static int MONSTERS_PER_PARTY = 3;
        public static int MAX_MONSTER_LEVEL = 100;
        public static int CHARACTERS_PER_BATTLE = 2;
        public static int MAX_CHARACTER_SKILLS = 3;

        public static void LoadSettingsFromSO(GameSettingsAsset set)
        {
            MONSTERS_PER_PARTY = set.monstersPerParty;
            MAX_MONSTER_LEVEL = set.maxMonsterLevel;
            CHARACTERS_PER_BATTLE = set.charactersPerBattle;
            MAX_CHARACTER_SKILLS = set.maxCharacterSkills;
        }
    }
}
