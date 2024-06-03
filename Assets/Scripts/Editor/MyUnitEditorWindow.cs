using RPGSystem;
using UnityEditor;

[CustomEditor(typeof(MyUnit))]
public class MyUnitEditorWindow : UnitEditorWindow
{
    protected override void OnGUI()
    {
        base.OnGUI();
    }

    protected override void GetSerializedProperties()
    {
        base.GetSerializedProperties();
    }
}
