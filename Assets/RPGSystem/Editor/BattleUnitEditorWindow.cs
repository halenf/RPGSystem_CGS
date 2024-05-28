using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BattleUnitEditorWindow : EditorWindow
{
    /*
    public void OnGUI()
    {
        // battle debug button
        if (GUILayout.Button(new GUIContent(showBattleDebug ? "Hide Battle Debug Info" : "Show Battle Debug Info", "Toggles the visibility of " +
            "data and tools relevant only when the unit is in a battle.")))
            showBattleDebug = !showBattleDebug;

        if (showBattleDebug)
        {
            // status slots layout for debugging
            EditorGUILayout.PropertyField(m_statusSlots, new GUIContent("Status Slots", "All the statuses " +
                "this unit is currently inflicted with. Only shows meaningful data during battles."));

            // stat modifiers display
            {
                GUIContent modifierFoldoutLabel = new GUIContent("Volatile Stat Modifiers", "Values representing " +
                    "the modifiers currently affecting this stat.");
                showModifierFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(showModifierFoldout, modifierFoldoutLabel);
                if (showModifierFoldout)
                {
                    string[] unitBaseStatNames = Enum.GetNames(typeof(UnitBaseStatNames));
                    for (int i = 1; i < 4; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(unitBaseStatNames[i], GUILayout.Width(width * 0.2f));
                        SerializedProperty statModifier = m_statModifiers.GetArrayElementAtIndex(i);
                        statModifier.floatValue = EditorGUILayout.Slider(statModifier.floatValue, 0, 2, GUILayout.Width(width * 0.25f));
                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.Space(20);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            // triggered effects display
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Triggered Effects", "The custom special statuses this unit is currently inflicted with."),
                    EditorStyles.boldLabel, GUILayout.Width(width * 0.2f));
                EditorGUILayout.PropertyField(m_triggeredEffects, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
    */
}
