using RPGSystem;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyStatus", menuName = "MyGame/Status")]
public class MyStatus : Status
{
    private List<BattleUnit> m_targets = new List<BattleUnit>();
    public List<BattleUnit> targets { get { return m_targets; } }

    public void AddTarget(BattleUnit target)
    {
        m_targets.Add(target);
    }
}
