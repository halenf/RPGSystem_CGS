using RPGSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyStatus", menuName = "MyGame/Status")]
public class MyStatus : Status
{
    private BattleUnit[] m_targets;
    public BattleUnit[] targets { get { return m_targets; } }

    public void SetTargets(BattleUnit[] targets)
    {
        m_targets = targets;
    }
}
