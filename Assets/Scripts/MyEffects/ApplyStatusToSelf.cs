using RPGSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "ApplyStatusToSelf", menuName = "MyGame/Effects/StatusToSelf", order = 1)]
public class ApplyStatusToSelf : Effect
{
    [SerializeField] private Status m_status;

    public override void DoEffect(BattleUnit user, BattleUnit target)
    {
        user.GainStatus(m_status);
        Debug.Log(user.displayName + " gained " + m_status.statusName + "!");
    }
}
