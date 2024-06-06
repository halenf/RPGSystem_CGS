using RPGSystem;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SkillCooldownEffect", menuName = "MyGame/Effects/SkillCooldown", order = 1)]
public class SkillCooldownEffect : Effect
{
    [SerializeField] private int m_targetSkillIndex;

    /// <summary>
    /// Value = Number of turns to add to Target's SkillSlot cooldowns.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="targets"></param>
    public override void DoEffect(BattleUnit user, BattleUnit target)
    {
        if (m_targetSkillIndex == -1)
        {
            for (int i = 0; i < target.skillSlots.Count; i++)
            {
                target.ChangeSkillCooldown(i, m_value);
                Debug.Log("All of " + target.displayName + "'s skill cooldowns is " + (m_value > 0 ? "extended" : "reduced") + " by " + m_value + " turn(s)!");
            }
        }
        else
        {
            target.ChangeSkillCooldown(m_targetSkillIndex, m_value);
            Debug.Log(target.displayName + " has " + target.skillSlots[m_targetSkillIndex].skill.skillName + 
                " cooldown " + (m_value > 0 ? "extended" : "reduced") + " by " + m_value + " turn(s)!");
        }
    }
}
