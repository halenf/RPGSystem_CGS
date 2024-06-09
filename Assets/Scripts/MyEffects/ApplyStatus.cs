using RPGSystem;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Status", menuName = "MyGame/Effects/ApplyStatus", order = 1)]
public class ApplyStatus : Effect
{
    [SerializeField] private Status m_status;
    
    public override void DoEffect(BattleUnit user, BattleUnit target)
    {
        m_status.SetUser(user);
        target.GainStatus(m_status);
        BattleTextLog.Instance.AddLine(target.displayName + " gained " + m_status.statusName + " for " + m_status.turnTimer + " turns!");
    }
}
