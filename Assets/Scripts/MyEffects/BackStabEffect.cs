using RPGSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "BackStab", menuName = "MyGame/Effects/Backstab")]
public class BackStabEffect : Effect
{
    [SerializeField] private EffectPower m_power;
    [SerializeField] private Status m_backstabBuff;

    private bool m_gotBuff = false;

    public override void DoEffect(BattleUnit user, BattleUnit target)
    {       
        target.TakeDamage((int)m_power);
        if (!m_gotBuff && target.battleID.character == user.battleID.character)
        {
            user.GainStatus(m_backstabBuff);
            m_gotBuff = true;
        }
    }
}
