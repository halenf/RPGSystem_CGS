using UnityEngine;
using UnityEngine.UI;
using RPGSystem;
using TMPro;
using static TMPro.TMP_Dropdown;

public class BattleTesterUI : MonoBehaviour
{
    private BattleTester m_battleTester;

    private MyUnit[] m_units;
    private Sprite[] m_sprites;

    [Header("UI Objects")]
    [SerializeField] private TMP_InputField m_nameField;
    [SerializeField] private TMP_Dropdown[] m_unitDropdowns = new TMP_Dropdown[3];
    [SerializeField] private TMP_Dropdown m_spriteDropdown;
    [SerializeField] private Button m_createCharacterButton;
    
    public void Initialise(MyUnit[] units, Sprite[] sprites)
    {
        m_units = units;
        m_sprites = sprites;

        // init unit dropdown options
        {
            OptionDataList options = new OptionDataList();
            for (int u = 0; u < m_units.Length; u++)
            {
                OptionData data = new OptionData();
                data.text = m_units[u].displayName;
                data.image = (m_units[u].unitData as MyUnitData).sprite;
                options.options.Add(data);
            }
            foreach (TMP_Dropdown dropdown in m_unitDropdowns)
                dropdown.options = options.options;
        }

        // init character sprite dropdown
        {
            OptionDataList options = new OptionDataList();
            for (int s = 0; s < m_sprites.Length; s++)
            {
                OptionData data = new OptionData();
                data.image = m_sprites[s];
                options.options.Add(data);
            }
            m_spriteDropdown.options = options.options;
        }
    }

    public void SetCharacterButton()
    {
        if (m_nameField.text != string.Empty)
            m_createCharacterButton.interactable = true;
        else
            m_createCharacterButton.interactable = false;
    }

    public void CreateCharacter()
    {
        if (m_nameField.text != string.Empty)
        { 
            Unit[] units = { m_units[m_unitDropdowns[0].value], m_units[m_unitDropdowns[1].value], m_units[m_unitDropdowns[2].value] };
            m_battleTester.StartBattle(m_nameField.text, units, m_sprites[m_spriteDropdown.value]);
        }
    }
}
