using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSystem;

public class GameManager : MonoBehaviour
{
    public BattleSceneUI sceneUI;
    
    // Start is called before the first frame update
    void Start()
    {
        sceneUI.Initialise(sceneUI.characterUI.character, sceneUI.characterUI.character);
    }

    // Update is called once per frame
    void Update()
    {
        sceneUI.UpdateUI();
    }
}
