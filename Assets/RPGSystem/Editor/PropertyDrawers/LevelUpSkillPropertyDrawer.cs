using UnityEditor;
using UnityEngine;

namespace RPGSystem
{
    [CustomPropertyDrawer(typeof(LevelUpSkill))]
    public class LevelUpSkillsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

            // calculate rects
            Rect levelRect = new Rect(position.x, position.y, position.width * 0.3f, position.height);
            Rect skillRect = new Rect(position.x + position.width * 0.335f, position.y, position.width * 0.6f, position.height);

            // draw fields
            float labelWidth = EditorGUIUtility.labelWidth; // save the label width to reset after
            EditorGUIUtility.labelWidth = Screen.width * 0.1f;
            EditorGUI.PropertyField(levelRect, property.FindPropertyRelative("m_level"));
            EditorGUIUtility.labelWidth = Screen.width * 0.1f;
            EditorGUI.PropertyField(skillRect, property.FindPropertyRelative("m_skill"));
            EditorGUIUtility.labelWidth = labelWidth; // reset label width

            EditorGUI.EndProperty();
        }
    }
}
