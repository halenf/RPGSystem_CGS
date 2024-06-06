using UnityEditor;
using UnityEngine;

namespace RPGSystem
{
    public abstract class BattleUnitPropertyDrawer : PropertyDrawer
    {
        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            OnInspectorGUI(position, property, label);

            EditorGUI.EndProperty();
        }

        protected virtual void OnInspectorGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.LabelField(new GUIContent(property.FindPropertyRelative("m_unit.unitName").stringValue));
        }
    }
}
