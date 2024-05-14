using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    [SerializeField] private GameSettings m_gameSettings;
    public GameSettings gameSettings
    {
        get { return m_gameSettings; }
    }

    public BattleSceneUI sceneUI;
    public Character character;

    // Start is called before the first frame update
    void Start()
    {
        if (current == null)
        {
            current = this;
            DontDestroyOnLoad(this);
        }
        
        sceneUI.Initialise(new Character[] { character, character });
    }

    // Update is called once per frame
    void Update()
    {
        sceneUI.UpdateUI();
    }
}
