using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigEffect : Effect
{
    [SerializeField] private MyStatus m_status;

    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {
        m_status.SetTargets(targets);
        user.GainStatus(m_status);
    }
}
