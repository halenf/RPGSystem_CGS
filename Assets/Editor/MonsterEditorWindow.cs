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
        
        int numOfSkills;
        int numOfStatuses;
        bool showStatusSlots = false;

        SerializedProperty m_monsterData;
        SerializedProperty m_monsterName;
        SerializedProperty m_skillSlots;
        SerializedProperty m_statusSlots;
        SerializedProperty m_statModifiers;
        SerializedProperty m_triggeredEffects;
        SerializedProperty m_exp;

        public void OnEnable()
        {
            monster = (Monster)target;
            numOfSkills = monster.skillSlots.Length;
            numOfStatuses = monster.statusSlots.Count;
            GetSerializedProperties();
        }

        public override void OnInspectorGUI()
        {
            
            // object slot for monster data
            EditorGUILayout.PropertyField(m_monsterData, new GUIContent("Base Monster Data", "What type of monster this is."));
            // set monster name
            // set default to monster data's name
            EditorGUILayout.PropertyField(m_monsterName, new GUIContent("Name", "The nickname of this monster. Defaults to the base monster's name."));
            if (monster.monsterData != null && m_monsterName.stringValue == string.Empty)
            {
                m_monsterName.stringValue = monster.monsterData.name;
            }

            // skill slot layout
            GUILayout.BeginHorizontal();
            GUILayout.Label("Skill Slots");
            GUILayout.Label("Skill");
            GUILayout.Label("Turn Timer");
            GUILayout.EndHorizontal();

            for (int i = 0; i < numOfSkills; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(("Skill Slot " + (i + 1).ToString()));
                SerializedProperty skillSlot = m_skillSlots.GetArrayElementAtIndex(i);
                SerializedProperty skill = skillSlot.FindPropertyRelative("m_skill");
                SerializedProperty turnTimer = skillSlot.FindPropertyRelative("m_turnTimer");
                EditorGUILayout.PropertyField(skill, GUIContent.none);
                EditorGUILayout.PropertyField(turnTimer, GUIContent.none);
                GUILayout.EndHorizontal();



                /*
                comp.skillSlots[i].skill = (Skill)EditorGUILayout.ObjectField(comp.skillSlots[i].skill, typeof(Skill), false);
                comp.skillSlots[i].turnTimer = EditorGUILayout.IntField(comp.skillSlots[i].turnTimer);
                */
            }

            // add and remove skill buttons
            GUILayout.BeginHorizontal();
            if (numOfSkills < 3)
                if (GUILayout.Button("+"))
                    numOfSkills++;
            if (numOfSkills > 1)
                if (GUILayout.Button("-"))
                {
                    monster.skillSlots[numOfSkills - 1].ClearSlot();
                    numOfSkills--;
                }
            GUILayout.EndHorizontal();

            // status slots layout for debugging
            GUIContent statusFoldoutLabel =  new GUIContent("Status Slots Debugging", "This menu will show all the statuses " +
                "this monster is currently inflicted with and allow you to add or remove statuses for debugging purposes.");
            showStatusSlots = EditorGUILayout.BeginFoldoutHeaderGroup(showStatusSlots, statusFoldoutLabel);
            if (showStatusSlots)
            {
                
                
                
                
                // add and remove status buttons
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("+"))
                    numOfStatuses++;
                if (numOfStatuses > 0)
                    if (GUILayout.Button("-"))
                    {
                        monster.statusSlots[numOfStatuses - 1].ClearSlot();
                        numOfStatuses--;
                    }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // apply changes to serialized object
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(20);

            base.OnInspectorGUI();

        }

        public void GetSerializedProperties()
        {
            m_monsterData = serializedObject.FindProperty("m_monsterData");
            m_monsterName = serializedObject.FindProperty("m_monsterName");
            m_skillSlots = serializedObject.FindProperty("m_skillSlots");
            m_statusSlots = serializedObject.FindProperty("m_statusSlots");
            m_statModifiers = serializedObject.FindProperty("m_statModifiers");
            m_triggeredEffects = serializedObject.FindProperty("m_triggeredEffects");
            m_exp = serializedObject.FindProperty("m_exp");
        }
    }
}
