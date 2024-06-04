using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "BackStab", menuName = "MyGame/Effects/Backstab")]
public class BackStabEffect : Effect
{
    [SerializeField] private EffectPower m_power;
    [SerializeField] private Status m_backstabBuff;

    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {       
        bool getBuff = false;
        foreach (BattleUnit target in targets)
        {
            target.TakeDamage((int)m_power);
            if (target.battleID.character == user.battleID.character)
                getBuff = true;
        }
        if (getBuff)
            user.GainStatus(m_backstabBuff);
    }
}
