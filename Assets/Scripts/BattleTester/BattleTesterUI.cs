using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPGSystem;

public class BattleTesterUI : MonoBehaviour
{
    private BattleTester m_battleTester;

    private MyUnit[] m_units;
    private Sprite[] m_sprites;

    [Header("UI Objects")]
    [SerializeField] private InputField m_nameField;
    [SerializeField] private Dropdown[] m_unitDropdowns = new Dropdown[3];
    [SerializeField] private Dropdown m_spriteDropdown;
    
    public void Initialise(MyUnit[] units, Sprite[] sprites)
    {
        m_units = units;
        m_sprites = sprites;
        
        for (int i = 0; i < m_unitDropdowns.Length; i++)
        {
            Dropdown.OptionDataList options = new Dropdown.OptionDataList();
            for (int u = 0; u < m_units.Length; u++)
            {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = m_units[u].displayName;
                //data.image = m_units[u]
                //options.options.Add(m_units[u]);

            }
        }
    }

    public void CreateCharacter()
    {
        Unit[] units = { m_units[m_unitDropdowns[0].value], m_units[m_unitDropdowns[1].value], m_units[m_unitDropdowns[2].value] };
        m_battleTester.CreateCharacter(m_nameField.text, units, m_sprites[m_spriteDropdown.value]);
    }
}
