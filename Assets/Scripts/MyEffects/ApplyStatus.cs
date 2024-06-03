using RPGSystem;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Status", menuName = "Effects/ApplyStatus", order = 1)]
public class ApplyStatus : Effect
{
    [SerializeField] private Status m_status;
    public Status status
    {
        get { return m_status; }
    }

    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {
        m_status.SetUser(user);
        foreach (BattleUnit target in targets)
        {
            target.GainStatus(m_status);
            Debug.Log(target.displayName + " gained " + m_status.statusName + " for " + m_status.turnTimer + " turns!");
        }
    }
}
