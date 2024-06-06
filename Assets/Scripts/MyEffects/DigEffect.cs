using RPGSystem;
using UnityEngine;

public class DigEffect : Effect
{
    [SerializeField] private MyStatus m_status;

    public override void DoEffect(BattleUnit user, BattleUnit target)
    {
        m_status.AddTarget(target);
        user.GainStatus(m_status);
    }
}
