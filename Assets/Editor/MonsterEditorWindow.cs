using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGSystem
{
    [CustomEditor(typeof(Monster), true)]
    public class MonsterEditorWindow : Editor
    {
        Monster comp;

        public void OnEnable()
        {
            comp = (Monster)target;
        }

        public override void OnInspectorGUI()
        {
            comp.monsterData = (MonsterData)EditorGUILayout.ObjectField("Monster Data", comp.monsterData, typeof(MonsterData), false);
            comp.monsterName = EditorGUILayout.TextField("Name", comp.monsterName);
            SkillSlot[] skillSlots = new SkillSlot[3];
            foreach (SkillSlot skillSlot in skillSlots)
            {

            }

            base.OnInspectorGUI();
        }
    }
}
