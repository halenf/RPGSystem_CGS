using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "StatusTimerEffect", menuName = "Effects/StatusTimer", order = 1)]
public class StatusTimerEffect : Effect
{
    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {
        foreach (BattleUnit target in targets)
        {
            for (int i = 0; i < target.statusSlots.Count; i++)
            {
                target.ChangeStatusTimer(i, m_value);
            }
            Debug.Log("All of " + target.displayName + "'s statuses were extended by " + m_value + " turns!");
        }
    }
}