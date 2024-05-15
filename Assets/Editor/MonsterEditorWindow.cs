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
        SerializedProperty m_monsterData, m_monsterName, m_totalExp, m_expToNextLevel, m_level, 
            m_skillSlots;

        public void OnEnable()
        {
            GetSerializedProperties();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            // object slot for monster data
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Base Monster Data", "What type of monster this is."), EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_monsterData, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }

            // don't show the rest of the ui if the monster has no monster data
            if (m_monsterData.objectReferenceValue != null)
            {
                // set monster name, set default to monster type name
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Nickname", "The nickname of this monster. Defaults to the base monster's name."), EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(m_monsterName, GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                    if (m_monsterName.stringValue == string.Empty)
                    {
                        m_monsterName.stringValue = ((MonsterData)m_monsterData.objectReferenceValue).monsterName;
                    }
                }

                // experience level display
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Level", "This monster's curent level."), EditorStyles.boldLabel, GUILayout.Width(Screen.width * 0.065f));
                    int inputLevel = EditorGUILayout.IntField(m_level.intValue, GUILayout.Width(Screen.width * 0.06f));
                    if (inputLevel != m_level.intValue)
                    {
                        ((Monster)serializedObject.targetObject).SetLevel(inputLevel);
                        //serializedObject.Update();
                    }
                    EditorGUILayout.LabelField(new GUIContent("Total Exp", "This monster's total experience."), EditorStyles.boldLabel, GUILayout.Width(Screen.width * 0.1f));
                    int inputExp = EditorGUILayout.IntField(m_totalExp.intValue, GUILayout.Width(Screen.width * 0.13f));
                    if (inputExp != m_totalExp.intValue)
                    {
                        ((Monster)serializedObject.targetObject).SetTotalExp(inputExp);
                        //serializedObject.Update();
                    }
                    // If the monster is at max level, don't show exp to next level
                    if (((Monster)target).level != GameSettings.MAX_MONSTER_LEVEL)
                    {
                        EditorGUILayout.LabelField(new GUIContent("Exp to Next Level", "The amount of exp this monster needs to level up."), EditorStyles.boldLabel, GUILayout.Width(Screen.width * 0.175f));
                        m_expToNextLevel.intValue = EditorGUILayout.IntField(m_expToNextLevel.intValue, GUILayout.Width(Screen.width * 0.12f));
                    }
                    EditorGUILayout.EndHorizontal();
                }

                // skill slot layout
                EditorGUILayout.PropertyField(m_skillSlots, new GUIContent("Skill Slots", "The monster's skill slots."));
                /*
                GUIContent skillFoldoutLabel = new GUIContent("Skill Slots", "This menu shows the monster's skill slots and allows you to " +
                    "add and remove skills.");
                showSkillSlots = EditorGUILayout.BeginFoldoutHeaderGroup(showSkillSlots, skillFoldoutLabel);
                if (showSkillSlots)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(Screen.width * 0.2f));
                    GUILayout.Label("Skill", GUILayout.Width(Screen.width * 0.35f));
                    GUILayout.Label("Turn Timer", GUILayout.Width(Screen.width * 0.35f));
                    GUILayout.EndHorizontal();

                    for (int i = 0; i < numOfSkills; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Slot " + (i + 1).ToString(), GUILayout.Width(Screen.width * 0.2f));
                        SerializedProperty skillSlot = m_skillSlots.GetArrayElementAtIndex(i);
                        SerializedProperty skill = skillSlot.FindPropertyRelative("m_skill");
                        SerializedProperty turnTimer = skillSlot.FindPropertyRelative("m_turnTimer");
                        EditorGUILayout.PropertyField(skill, GUIContent.none, GUILayout.Width(Screen.width * 0.35f));
                        EditorGUILayout.PropertyField(turnTimer, GUIContent.none, GUILayout.Width(Screen.width * 0.35f));
                        GUILayout.EndHorizontal();
                    }

                    // add and remove skill buttons
                    GUILayout.BeginHorizontal();

                    if (numOfSkills < 3)
                    {
                        if (GUILayout.Button("+", GUILayout.Width(Screen.width * 0.1f)))
                        {
                            monster.AddSkillSlot();
                            numOfSkills++;
                        }
                    }
                    else
                        EditorGUILayout.LabelField("", GUILayout.Width(Screen.width * 0.1f));

                    if (numOfSkills > 1)
                        if (GUILayout.Button("-", GUILayout.Width(Screen.width * 0.1f)))
                        {
                            monster.RemoveSkillSlot();
                            numOfSkills--;
                        }
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                */
            }

            // reset monster button
            GUILayout.Space(20);
            if (GUILayout.Button("Reset Monster"))
                ((Monster)serializedObject.targetObject).ResetMonsterData();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                GetSerializedProperties(); // If changes were made to the object, update references to properties
                serializedObject.Update(); // appears lists need their reference updated after they're changed
            }

            //base.OnInspectorGUI();
        }

        public void GetSerializedProperties()
        {
            m_monsterData = serializedObject.FindProperty("m_monsterData");
            m_monsterName = serializedObject.FindProperty("m_monsterName");
            m_totalExp = serializedObject.FindProperty("m_totalExp");
            m_expToNextLevel = serializedObject.FindProperty("m_expToNextLevel");
            m_level = serializedObject.FindProperty("m_level");
            m_skillSlots = serializedObject.FindProperty("m_skillSlots");
        }
    }
}
