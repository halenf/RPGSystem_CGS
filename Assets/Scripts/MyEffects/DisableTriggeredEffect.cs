using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DisableTriggeredEffect", menuName = "MyGame/Effects/DisableTriggeredEffect", order = 1)]
public class DisableTriggeredEffect : Effect
{
    // the effects to be enabled
    [SerializeField] private TriggeredEffect m_effect;

    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {
        foreach (BattleUnit target in targets)
        {
            target.DisableTriggeredEffect(m_effect);
            Debug.Log(target.displayName + " recovered from " + m_effect + "!");
        }
    }
}
