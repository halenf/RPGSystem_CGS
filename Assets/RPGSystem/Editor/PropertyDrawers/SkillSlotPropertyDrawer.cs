using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RPGSystem
{
    [CustomPropertyDrawer(typeof(SkillSlot))]
    public class SkillSlotPropertyDrawer : PropertyDrawer
    {       
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

            // calculate rects
            Rect skillRect = new Rect(position.x, position.y, position.width * 0.5f, position.height);
            Rect turnTimerRect = new Rect(position.x + position.width * 0.525f, position.y, position.width * 0.475f, position.height);

            // draw fields
            float labelWidth = EditorGUIUtility.labelWidth; // save the label width to reset after
            EditorGUIUtility.labelWidth = Screen.width * 0.1f;
            EditorGUI.PropertyField(skillRect, property.FindPropertyRelative("m_skill"));
            EditorGUIUtility.labelWidth = Screen.width * 0.15f;
            EditorGUI.PropertyField(turnTimerRect, property.FindPropertyRelative("m_turnTimer"));
            EditorGUIUtility.labelWidth = labelWidth; // reset label width

            EditorGUI.EndProperty();
        }
    }
}

