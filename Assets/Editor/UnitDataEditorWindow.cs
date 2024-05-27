using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPGSystem
{
    //[CustomEditor(typeof(UnitData), true)]
    public class UnitDataEditorWindow : Editor
    {
        SerializedProperty m_unitName, m_baseStats;

        public void OnEnable()
        {
            GetSerializedProperties();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(m_unitName);
            EditorGUILayout.PropertyField(m_baseStats);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                GetSerializedProperties();
                serializedObject.Update();
            }
        }

        public void GetSerializedProperties()
        {
            m_unitName = serializedObject.FindProperty("m_unitName");
            m_baseStats = serializedObject.FindProperty("m_baseStats");
        }
    }
}
