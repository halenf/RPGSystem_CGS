using UnityEditor;
using UnityEngine;

namespace RPGSystem
{
    public abstract class UnitEditorWindow : Editor
    {
        protected SerializedProperty m_unitData, m_unitNickname, m_totalExp, m_expToNextLevel, m_level, m_skillSlots;

        private void OnEnable()
        {
            GetSerializedProperties();
        }

        public sealed override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            OnGUI();

            // reset unit button
            GUILayout.Space(10);
            if (GUILayout.Button("Reset Unit"))
                (target as Unit).ResetUnit();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                GetSerializedProperties(); // If changes were made to the object, update references to properties
            }
        }

        protected virtual void OnGUI()
        {
            // object slot for unit data
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Base Unit Data", "What type of unit this is."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_unitData, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            //Unit unit = (Unit)serializedObject.targetObject;
            Unit unit = target as Unit;

            // don't show the rest of the ui if the unit has no unit data
            if (unit.unitData != null)
            {
                // set unit name, set default to unit type name
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Nickname", "The nickname of this unit. Defaults to the base Unit's name."), EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_unitNickname, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                if (m_unitNickname.stringValue == string.Empty)
                {
                    m_unitNickname.stringValue = unit.unitData.unitName;
                }

                // experience/level display
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Level", "This unit's curent level."), EditorStyles.boldLabel, GUILayout.Width(Screen.width * 0.08f));
                int inputLevel = EditorGUILayout.IntField(m_level.intValue, GUILayout.Width(Screen.width * 0.1f));
                if (inputLevel != m_level.intValue)
                    unit.SetLevel(inputLevel);

                EditorGUILayout.LabelField(new GUIContent("Total Exp", "This unit's total experience."), EditorStyles.boldLabel, GUILayout.Width(Screen.width * 0.13f));
                int inputExp = EditorGUILayout.IntField(m_totalExp.intValue, GUILayout.Width(Screen.width * 0.18f));
                if (inputExp != m_totalExp.intValue)
                    unit.SetTotalExp(inputExp);

                // If the unit is at max level, don't show exp to next level
                if (unit.level != GameSettings.MAX_UNIT_LEVEL)
                {
                    EditorGUILayout.LabelField(new GUIContent("Exp to Next Level", "The amount of exp this unit needs to level up."), EditorStyles.boldLabel, GUILayout.Width(Screen.width * 0.235f));
                    m_expToNextLevel.intValue = EditorGUILayout.IntField(m_expToNextLevel.intValue, GUILayout.Width(Screen.width * 0.16f));
                }
                EditorGUILayout.EndHorizontal();

                // Stats display
                EditorGUILayout.LabelField(new GUIContent("Stats"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                for (int i = 0; i < GameSettings.STAT_NAMES.Length; i++)
                {
                    EditorGUILayout.LabelField(new GUIContent(GameSettings.STAT_NAMES[i] + ": " + unit.GetStat((BaseStatName)i)));
                }
                EditorGUI.indentLevel--;

                // skill slot layout
                // if there aren't any SkillSlots, then initialise the Unit with its default Skills at its current level.
                if (m_skillSlots.arraySize == 0)
                {
                    unit.InitialiseSkillSlots();
                }

                EditorGUILayout.PropertyField(m_skillSlots, new GUIContent("Skill Slots", "The unit's skill slots."));

                // Manually initialise skill slots
                if (GUILayout.Button("Re-initialise Skill Slots"))
                    unit.InitialiseSkillSlots();
            }
        }

        protected virtual void GetSerializedProperties()
        {
            m_unitData = serializedObject.FindProperty("m_unitData");
            m_unitNickname = serializedObject.FindProperty("m_unitNickname");
            m_totalExp = serializedObject.FindProperty("m_totalExp");
            m_expToNextLevel = serializedObject.FindProperty("m_expToNextLevel");
            m_level = serializedObject.FindProperty("m_level");
            m_skillSlots = serializedObject.FindProperty("m_skillSlots");
        }
    }
}
