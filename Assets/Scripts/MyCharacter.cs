using RPGSystem;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MyCharacter", menuName = "Character", order = 1)]
public class MyCharacter : Character
{
    [System.Serializable]   
    public class CharacterSkillSlot
    {
        [SerializeField] private Skill m_skill;
        [SerializeField] private int m_cost;

        public Skill skill
        { get { return m_skill; } }
        public int cost
        { get { return m_cost; } }

        public void ResetSlot()
        {
            m_skill = null;
            m_cost = 0;
        }
    }

    /// <summary>
    /// 2D image representing the Character.
    /// </summary>
    [SerializeField] private Sprite m_sprite;

    /// <summary>
    /// Character's available character skills.
    /// </summary>
    [Header("Character Skills")]
    [SerializeField] private int m_skill;
    [SerializeField] private int m_currentAP;
    [SerializeField] private CharacterSkillSlot[] m_characterSkillSlots = new CharacterSkillSlot[GameSettings.MAX_SKILLS_PER_CHARACTER];

    public Sprite sprite { get { return m_sprite; } }
    public int skill { get { return m_skill; } }
    public int maxAP { get { return (int)(20.0f / (float)Mathf.Log(m_skill) + 10.0f); } }
    public int currentAP { get { return m_currentAP; } }
    public CharacterSkillSlot[] characterSkillSlots { get { return m_characterSkillSlots; } }

    public override void InitialiseForBattle()
    {
        base.InitialiseForBattle();
        m_currentAP = maxAP;
        m_characterSkillSlots = m_characterSkillSlots.Where(slot => slot.skill != null).ToArray();
    }

    public override void ResetCharacter()
    {
        base.ResetCharacter();
        m_sprite = null;
        m_skill = 1;
        m_characterSkillSlots = null;
    }

    public void ChangeAP(int value)
    {
        m_currentAP += value;

        // keep AP within 0 and max
        if (m_currentAP > maxAP)
            m_currentAP = maxAP;
        if (m_currentAP < 0)
            m_currentAP = 0;
    }
}
