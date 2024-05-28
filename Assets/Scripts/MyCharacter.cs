using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacter : Character
{
    [CreateAssetMenu(fileName = "MyCharacter", menuName = "Character", order = 1)]
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
    [SerializeField] protected Sprite m_sprite;

    /// <summary>
    /// Character's available character skills.
    /// </summary>
    [SerializeField] protected CharacterSkillSlot[] m_characterSkillSlots = new CharacterSkillSlot[GameSettings.MAX_SKILLS_PER_CHARACTER];

    protected int m_skill;
    protected int m_currentAP;

    public Sprite sprite { get { return m_sprite; } }
    public int skill { get { return m_skill; } }
    public int maxAP { get { return (int)(20.0f / (float)Mathf.Log(m_skill) + 10.0f); } }
    public int currentAP { get { return m_currentAP; } }
    public CharacterSkillSlot[] characterSkillSlots { get { return m_characterSkillSlots; } }

    public override void InitialiseForBattle()
    {
        base.InitialiseForBattle();
        m_currentAP = maxAP;
    }

    public override void ResetCharacter()
    {
        base.ResetCharacter();
        m_sprite = null;
        m_skill = 1;
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
