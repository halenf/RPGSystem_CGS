using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    [SerializeField] private GameSettingsAsset m_gameSettingsAsset;

    // Start is called before the first frame update
    void Awake()
    {
        if (current == null)
        {
            current = this;
            DontDestroyOnLoad(this);
        }

        // Load the game settings
        LoadGameSettings();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("Load Settings")]
    public void LoadGameSettings()
    {
        // load game settings from asset
        if (m_gameSettingsAsset)
            GameSettings.LoadSettingsFromSO(m_gameSettingsAsset);
    }
}
