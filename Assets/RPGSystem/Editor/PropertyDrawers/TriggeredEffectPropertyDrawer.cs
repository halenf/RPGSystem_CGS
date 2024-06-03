using UnityEditor;
using UnityEngine;

namespace RPGSystem
{
    [CustomPropertyDrawer(typeof(TriggeredEffect))]
    public class TriggeredEffectPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            uint value = (uint)EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
            if (EditorGUI.EndChangeCheck())
                property.intValue = (int)value;
        }
    }
}

