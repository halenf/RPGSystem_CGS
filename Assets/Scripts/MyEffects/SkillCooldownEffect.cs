using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SkillCooldownEffect", menuName = "Effects/SkillCooldown", order = 1)]
public class SkillCooldownEffect : Effect
{
    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {
        foreach (BattleUnit target in targets)
        {
            for (int i = 0; i < 3; i++)
            {
                target.ChangeSkillCooldown(i, m_value);
            }
        }
    }
}