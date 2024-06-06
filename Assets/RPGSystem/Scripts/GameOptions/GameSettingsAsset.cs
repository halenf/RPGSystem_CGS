using UnityEngine;

namespace RPGSystem
{
    [CreateAssetMenu(fileName = "Settings", menuName = "RPGSystem/Game Settings", order = 0)]
    public class GameSettingsAsset : ScriptableObject
    {
        [Header("Core Settings")]
        [SerializeField] protected int m_unitsPerParty = 3;
        [SerializeField] protected int m_maxUnitLevel = 100;
        [SerializeField] protected int m_charactersPerBattle = 2;
        [SerializeField] protected int m_maxSkillsPerUnit = 3;
        [SerializeField] protected float m_maxStatModifier = 2;

        [SerializeField] protected string[] m_statNames = { "Max HP", "Attack", "Defence", "Speed" };

        // core settings accessors
        public int unitsPerParty { get { return m_unitsPerParty; } }
        public int maxUnitLevel { get { return m_maxUnitLevel; } }
        public int charactersPerBattle { get { return m_charactersPerBattle; } }
        public int maxSkillsPerUnit { get { return m_maxSkillsPerUnit; } }
        public float maxStatModifier { get { return m_maxStatModifier; } }
        public string[] statNames { get { return m_statNames; } }
    }
}
