using RPGSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "MyUnitData", menuName = "MyGame/UnitData", order = 1)]
public class MyUnitData : UnitData
{
    public enum UnitLevelCurve
    {
        Slow = 220,
        Medium = 200,
        Fast = 185
    }

    // The value that determines how the unit grows
    [SerializeField] private Sprite m_sprite;
    [SerializeField] private UnitLevelCurve m_levelCurve;
    public Sprite sprite { get { return m_sprite; } }
    public UnitLevelCurve levelCurve { get { return m_levelCurve; } }

    public override void ResetUnitData()
    {
        base.ResetUnitData();
        m_levelCurve = default;
        m_levelUpSkills = null;
    }
}
