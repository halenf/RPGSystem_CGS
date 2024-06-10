using UnityEngine;

using RPGSystem;

public class BattleTester : MonoBehaviour
{
    [Header("Object Prefabs")]
    [SerializeField] private BattleTesterUI m_uiPrefab;
    private BattleTesterUI m_ui;
    [SerializeField] private MyBattleScene m_battleScenePrefab;
    private MyBattleScene m_battleScene;

    [Space]

    [SerializeField] private MyCharacter[] m_presetCharacters;

    [Header("Character Building Options")]
    [SerializeField] private Sprite[] m_spriteOptions;
    [SerializeField] private MyUnit[] m_unitOptions;

    private MyCharacter m_character;

    private void Start()
    {
        InitialiseUIObject();
        m_battleScene = null;
        m_character = null;
    }

    private void Update()
    {
        // if battle scene exists and has stopped playing
        if (m_battleScene != null && !m_battleScene.isPlaying)
        {
            Destroy(m_battleScene);
            InitialiseUIObject();
        }
    }

    public void StartBattle(string name, Unit[] units, Sprite sprite)
    {
        m_character = ScriptableObject.CreateInstance<MyCharacter>();
        m_character.Initialise(name, units, sprite);
        Destroy(m_ui);
        m_battleScene = Instantiate(m_battleScenePrefab);
        m_battleScene.Initialise(new Character[] { m_character, m_presetCharacters[Random.Range(0, m_presetCharacters.Length)] });
    }

    private void InitialiseUIObject()
    {
        m_ui = Instantiate(m_uiPrefab);
        m_ui.Initialise(m_unitOptions, m_spriteOptions);
    }
}
