using RPGSystem;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "StatEffect", menuName = "MyGame/Effects/StatModifier", order = 1)]
public class StatModifierEffect : Effect
{
    // note: HP cannot be boosted via this method
    [SerializeField] private BaseStatName m_baseStat;

    /// <summary>
    /// Value = Percentage value to apply to chosen Stat. Negative values represent a drop.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="targets"></param>
    public override void DoEffect(BattleUnit user, BattleUnit target)
    {
        if (m_baseStat == BaseStatName.Health)
            return;

        target.ApplyStatModifier(m_baseStat, m_value);
        Debug.Log(target.displayName + "'s " + GameSettings.STAT_NAMES[(int)m_baseStat] + " is " + (m_value > 0 ? "raised" : "dropped") + " by " + m_value + "%!");
    }
}
