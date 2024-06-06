using RPGSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "DisableTriggeredEffect", menuName = "MyGame/Effects/DisableTriggeredEffect", order = 1)]
public class DisableTriggeredEffect : Effect
{
    // the effects to be enabled
    [SerializeField] private TriggeredEffect m_effect;

    public override void DoEffect(BattleUnit user, BattleUnit target)
    {
        target.DisableTriggeredEffect(m_effect);
        Debug.Log(target.displayName + " recovered from " + m_effect + "!");
    }
}
