using RPGSystem;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MyCharacter", menuName = "MyGame/Character", order = 1)]
public class MyCharacter : Character
{
    /// <summary>
    /// 2D image representing the Character.
    /// </summary>
    [SerializeField] private Sprite m_sprite;

    public Sprite sprite { get { return m_sprite; } }

    public bool hasUnitsForBattle
    {
        get
        {
            int dead = 0;
            foreach (Unit unit in m_units)
            {
                if (unit.currentHP <= 0)
                    dead++;
            }
            if (dead == m_units.Length)
                return false;
            else
                return true;
        }
    }

    public void Initialise(string name, Unit[] units, Sprite sprite)
    {
        m_characterName = name;
        m_units = units;
        m_sprite = sprite;
    }

    public override void InitialiseForBattle()
    {
        base.InitialiseForBattle();
    }

    public override void ResetCharacter()
    {
        base.ResetCharacter();
        m_sprite = null;
    }
}
