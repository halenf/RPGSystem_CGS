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
    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {
        foreach (BattleUnit target in targets)
        {
            for (int i = 0; i < target.statusSlots.Count; i++)
            {
                if (m_statusName == string.Empty)
                    target.ChangeStatusTimer(i, m_value);
                else if (target.statusSlots[i].status.statusName == m_statusName)
                {
                    target.ChangeStatusTimer(i, m_value);
                    Debug.Log(target.displayName + "'s " + m_statusName + " was " + (m_value > 0 ? "extended" : "reduced") + " by " + m_value + " turns!");
                    return;
                }
            }
            if (m_statusName == string.Empty)
                Debug.Log("All of " + target.displayName + "'s statuses were " + (m_value > 0 ? "extended" : "reduced") + " by " + m_value + " turn(s)!");
        }
    }
}
