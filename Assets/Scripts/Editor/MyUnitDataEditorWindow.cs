using RPGSystem;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MyUnitData))]
public class MyUnitDataEditorWindow : UnitDataEditorWindow
{
    private SerializedProperty m_levelCurve;
    
    protected override void OnGUI()
    {
        base.OnGUI();
        EditorGUILayout.LabelField(new GUIContent("Custom UnitData Fields"), EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_levelCurve);
    }

    protected override void GetSerializedProperties()
    {
        base.GetSerializedProperties();
        m_levelCurve = serializedObject.FindProperty("m_levelCurve");
    }
}
