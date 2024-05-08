using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSystem;

public class GameManager : MonoBehaviour
{
    public BattleSceneUI sceneUI;
    public Character character;
    
    // Start is called before the first frame update
    void Start()
    {
        sceneUI.Initialise(character, character);
    }

    // Update is called once per frame
    void Update()
    {
        sceneUI.UpdateUI();
    }
}
