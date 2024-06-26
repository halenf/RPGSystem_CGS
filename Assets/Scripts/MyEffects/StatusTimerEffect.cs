using RPGSystem;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "StatusTimerEffect", menuName = "MyGame/Effects/StatusTimer", order = 1)]
public class StatusTimerEffect : Effect
{
    [SerializeField] private string m_statusName;
    
    /// <summary>
    /// Value = number of turns to add to targets' Status Slots.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="targets"></param>
    public override void DoEffect(BattleUnit user, BattleUnit target)
    {
        for (int i = 0; i < target.statusSlots.Count; i++)
        {
            if (m_statusName == string.Empty)
            {
                target.ChangeStatusTimer(i, m_value);
            }
            else if (target.statusSlots[i].status.statusName == m_statusName)
            {
                target.ChangeStatusTimer(i, m_value);
                BattleTextLog.Instance.AddLine(target.displayName + "'s " + m_statusName + " was " + (m_value > 0 ? "extended" : "reduced") + " by " + m_value + " turns!");
                break;
            }
        }
        if (m_statusName == string.Empty)
            BattleTextLog.Instance.AddLine("All of " + target.displayName + "'s statuses were " + (m_value > 0 ? "extended" : "reduced") + " by " + m_value + " turn(s)!");
        if (target.statusSlots.Count == 0)
            BattleTextLog.Instance.AddLine("But " + target.displayName + " is not inflicted with any Statuses!");

        // check if any of the statuses should be cleared
        target.CheckStatusTimers();
    }
}
