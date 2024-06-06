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

        LoadGameSettings();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Loads the currently held GameSettingsAsset into the static GameSettings class.
    /// </summary>
    [ContextMenu("Load Settings")]
    public void LoadGameSettings()
    {
        // load game settings from asset
        if (m_gameSettingsAsset)
            GameSettings.LoadSettingsFromSO(m_gameSettingsAsset);
        else
            Debug.LogError("No GameSettingsAsset attached to Manager.");
    }
}
