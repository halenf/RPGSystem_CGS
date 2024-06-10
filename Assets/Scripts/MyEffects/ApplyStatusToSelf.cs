using RPGSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "ApplyStatusToSelf", menuName = "MyGame/Effects/StatusToSelf", order = 1)]
public class ApplyStatusToSelf : Effect
{
    [SerializeField] private Status m_status;

    public override void DoEffect(BattleUnit user, BattleUnit target)
    {
        m_status.SetUser(user);
        user.GainStatus(m_status);
        BattleTextLog.Instance.AddLine(user.displayName + " gained " + m_status.statusName +" for " + m_status.turnTimer + " turns!");
    }
}
