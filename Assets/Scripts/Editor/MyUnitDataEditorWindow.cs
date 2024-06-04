using RPGSystem;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MyUnitData))]
public class MyUnitDataEditorWindow : UnitDataEditorWindow
{
    private SerializedProperty m_sprite, m_levelCurve;
    
    protected override void OnGUI()
    {
        base.OnGUI();
        EditorGUILayout.LabelField(new GUIContent("Custom UnitData Fields"), EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_sprite);
        EditorGUILayout.PropertyField(m_levelCurve);
    }

    protected override void GetSerializedProperties()
    {
        base.GetSerializedProperties();
        m_sprite = serializedObject.FindProperty("m_sprite");
        m_levelCurve = serializedObject.FindProperty("m_levelCurve");
    }
}
