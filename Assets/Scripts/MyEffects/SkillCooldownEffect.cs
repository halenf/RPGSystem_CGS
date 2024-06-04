using RPGSystem;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SkillCooldownEffect", menuName = "MyGame/Effects/SkillCooldown", order = 1)]
public class SkillCooldownEffect : Effect
{
    /// <summary>
    /// Value = Number of turns to add to Target's SkillSlot cooldowns.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="targets"></param>
    public override void DoEffect(BattleUnit user, BattleUnit[] targets)
    {
        foreach (BattleUnit target in targets)
        {
            for (int i = 0; i < 3; i++)
            {
                target.ChangeSkillCooldown(i, m_value);
                Debug.Log("All of " + target.displayName + "'s skill cooldowns were " + (m_value > 0 ? "extended" : "reduced") + " by " + m_value + " turn(s)!");
            }
        }
    }
}
