using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EnableTriggeredEffect", menuName = "Effects/EnableTriggeredEffect", order = 1)]
public class EnableTriggeredEffect : Effect
{
    // the effects to be enabled
    [SerializeField] private TriggeredEffect m_effect;
    public TriggeredEffect effect
    {
        get { return m_effect; }
    }

    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {
        foreach (BattleUnit target in targets)
            target.EnableTriggeredEffect(effect);
    }
}
