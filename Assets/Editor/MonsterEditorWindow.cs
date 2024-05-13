using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPGSystem
{
    [CustomEditor(typeof(Monster), true)]
    public class MonsterEditorWindow : Editor
    {
        Monster monster;

        bool showBattleDebug = false;
        bool showModifierFoldout = false;

        SerializedProperty m_monsterData;
        SerializedProperty m_monsterName;
        SerializedProperty m_skillSlots;
        SerializedProperty m_statusSlots;
        SerializedProperty m_statModifiers;
        SerializedProperty m_triggeredEffects;
        SerializedProperty m_exp;

        float width;

        public void OnEnable()
        {
            monster = (Monster)target;
            GetSerializedProperties();
            width = Screen.width;
        }

        public override void OnInspectorGUI()
        {
            // object slot for monster data
            EditorGUILayout.PropertyField(m_monsterData, new GUIContent("Base Monster Data", "What type of monster this is."));

            // don't show the rest of the ui if the monster has no monster data
            if (m_monsterData.objectReferenceValue == null)
                return;

            // set monster name, set default to monster type name
            EditorGUILayout.PropertyField(m_monsterName, new GUIContent("Nickname", "The nickname of this monster. Defaults to the base monster's name."));
            if (m_monsterName.stringValue == string.Empty)
            {
                m_monsterName.stringValue = ((MonsterData)m_monsterData.objectReferenceValue).monsterName;
            }

            // experience level display
            EditorGUILayout.BeginHorizontal();
            m_exp.intValue = EditorGUILayout.IntField(new GUIContent("Exp", "This monster's total experience."), m_exp.intValue);
            // if the user changed the level field
            int inputLevel = EditorGUILayout.IntField(new GUIContent("Level", "This monster's curent level."), monster.level);
            if (inputLevel != monster.level)
            {
                monster.level = inputLevel;
                serializedObject.Update();
            }
            EditorGUILayout.EndHorizontal();

            // skill slot layout
            EditorGUILayout.PropertyField(m_skillSlots, new GUIContent("Skill Slots", "The monster's skill slots."));
            /*
            GUIContent skillFoldoutLabel = new GUIContent("Skill Slots", "This menu shows the monster's skill slots and allows you to " +
                "add and remove skills.");
            showSkillSlots = EditorGUILayout.BeginFoldoutHeaderGroup(showSkillSlots, skillFoldoutLabel);
            if (showSkillSlots)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(width * 0.2f));
                GUILayout.Label("Skill", GUILayout.Width(width * 0.35f));
                GUILayout.Label("Turn Timer", GUILayout.Width(width * 0.35f));
                GUILayout.EndHorizontal();

                for (int i = 0; i < numOfSkills; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Slot " + (i + 1).ToString(), GUILayout.Width(width * 0.2f));
                    SerializedProperty skillSlot = m_skillSlots.GetArrayElementAtIndex(i);
                    SerializedProperty skill = skillSlot.FindPropertyRelative("m_skill");
                    SerializedProperty turnTimer = skillSlot.FindPropertyRelative("m_turnTimer");
                    EditorGUILayout.PropertyField(skill, GUIContent.none, GUILayout.Width(width * 0.35f));
                    EditorGUILayout.PropertyField(turnTimer, GUIContent.none, GUILayout.Width(width * 0.35f));
                    GUILayout.EndHorizontal();
                }

                // add and remove skill buttons
                GUILayout.BeginHorizontal();

                if (numOfSkills < 3)
                {
                    if (GUILayout.Button("+", GUILayout.Width(width * 0.1f)))
                    {
                        monster.AddSkillSlot();
                        numOfSkills++;
                    }
                }
                else
                    EditorGUILayout.LabelField("", GUILayout.Width(width * 0.1f));

                if (numOfSkills > 1)
                    if (GUILayout.Button("-", GUILayout.Width(width * 0.1f)))
                    {
                        monster.RemoveSkillSlot();
                        numOfSkills--;
                    }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            */

            // battle debug button
            if (GUILayout.Button(new GUIContent(showBattleDebug ? "Hide Battle Debug Info" : "Show Battle Debug Info", "Toggles the visibility of " +
                "data and tools relevant only when the monster is in a battle.")))
                showBattleDebug = !showBattleDebug;

            if (showBattleDebug)
            {
                // status slots layout for debugging
                EditorGUILayout.PropertyField(m_statusSlots, new GUIContent("Status Slots Debugging", "All the statuses " +
                    "this monster is currently inflicted with. Only shows meaningful data during battles."));

                // stat modifiers display
                GUIContent modifierFoldoutLabel = new GUIContent("Volatile Stat Modifiers", "Values representing " +
                    "the modifiers currently affecting this stat.");
                showModifierFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(showModifierFoldout, modifierFoldoutLabel);
                if (showModifierFoldout)
                {
                    string[] monsterBaseStatNames = Enum.GetNames(typeof(MonsterBaseStats));
                    for (int i = 1; i < 4; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(monsterBaseStatNames[i], GUILayout.Width(width * 0.2f));
                        SerializedProperty statModifier = m_statModifiers.GetArrayElementAtIndex(i);
                        statModifier.floatValue = EditorGUILayout.Slider(statModifier.floatValue, 0, 2, GUILayout.Width(width * 0.25f));
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            // apply changes to serialized object
            if (serializedObject.ApplyModifiedProperties())
                // If changes were made to the object, update references to properties
                GetSerializedProperties();

            EditorGUILayout.Space(30);

            //base.OnInspectorGUI();
        }

        public void GetSerializedProperties()
        {
            m_monsterData = serializedObject.FindProperty("m_monsterData");
            m_monsterName = serializedObject.FindProperty("m_monsterName");
            m_exp = serializedObject.FindProperty("m_exp");
            m_skillSlots = serializedObject.FindProperty("m_skillSlots");
            m_statusSlots = serializedObject.FindProperty("m_statusSlots");
            m_statModifiers = serializedObject.FindProperty("m_statModifiers");
            m_triggeredEffects = serializedObject.FindProperty("m_triggeredEffects");
        }
    }
}
