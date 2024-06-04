using RPGSystem;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EnableTriggeredEffect", menuName = "MyGame/Effects/EnableTriggeredEffect", order = 1)]
public class EnableTriggeredEffect : Effect
{
    // the effects to be enabled
    [SerializeField] private TriggeredEffect m_effect;

    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {
        foreach (BattleUnit target in targets)
        {
            target.EnableTriggeredEffect(m_effect);
            Debug.Log(target.displayName + " gained " + m_effect.ToString() + "!");
        }
    }
}
