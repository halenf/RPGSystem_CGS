using UnityEditor;
using UnityEngine;

namespace RPGSystem
{
    public abstract class UnitDataEditorWindow : Editor
    {
        protected SerializedProperty m_unitName, m_baseStats, m_levelUpSkills;

        private bool showBaseStatFoldout;

        private void OnEnable()
        {
            GetSerializedProperties();
        }

        public sealed override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            OnGUI();

            // reset unit button
            GUILayout.Space(20);
            if (GUILayout.Button("Reset UnitData"))
                ((UnitData)serializedObject.targetObject).ResetUnitData();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                GetSerializedProperties();
                serializedObject.Update();
            }
        }

        protected virtual void OnGUI()
        {
            EditorGUILayout.PropertyField(m_unitName);

            showBaseStatFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(showBaseStatFoldout, new GUIContent("Base Stats"));
            // show base stat field
            if (showBaseStatFoldout)
            {
                int width = Screen.width;
                EditorGUI.indentLevel++;
                for (int i = 0; i < m_baseStats.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent(((BaseStatName)i).ToString()), GUILayout.Width(width * 0.16f));
                    EditorGUILayout.PropertyField(m_baseStats.GetArrayElementAtIndex(i).FindPropertyRelative("m_value"), GUIContent.none, GUILayout.Width(width * 0.13f));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.PropertyField(m_levelUpSkills);
        }

        protected virtual void GetSerializedProperties()
        {
            m_unitName = serializedObject.FindProperty("m_unitName");
            m_baseStats = serializedObject.FindProperty("m_baseStats");
            m_levelUpSkills = serializedObject.FindProperty("m_levelUpSkills");
        }
    }
}
