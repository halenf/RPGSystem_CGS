using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    [CreateAssetMenu(fileName = "Settings", menuName = "RPGSystem/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] public int monstersPerParty;
        [SerializeField] public int maxMonsterLevel;
        [SerializeField] public int charactersPerBattle;
    }
}
