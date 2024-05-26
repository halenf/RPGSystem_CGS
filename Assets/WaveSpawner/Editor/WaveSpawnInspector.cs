using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AdamAssets
{
    [CustomEditor(typeof(WaveSpawning))]
    public class WaveSpawnInspector : Editor
    {
        // Since I'm using a complex system to render out the waves in this custom editor
        //  I need to save a version of waves seperately from the original inside WaveSpawning.
        List<WaveSpawning.Wave> waves;

        public override void OnInspectorGUI()
        {
            // Note: 
            // There are generally 2 ways to handle variables in a custom inspector:
            // The first is with SerializedProperty, this is what is reccommended
            // for most use cases.
            //
            // For anything especially complex like the wave system,
            // you can also use "targets".
            // targets has a few small issues here and there, but one of the most obvious 
            //  is that it can only work with public variables.


            //------ Serialized Property ------//
            //
            // Here is a quick example for a Transform inspector field.
            // serializedObject is a variable that can reference the script that this editor
            //  is attached to.
            // serializedObject.FindProperty will search the script for a variable with
            //  a matching name.
            SerializedProperty spawnPoint = serializedObject.FindProperty("spawnPoint");

            // There are different EditorGUILayout functions for most basic types
            //  like float or string, but for Objects like Transform, we have ObjectField.
            spawnPoint.objectReferenceValue = EditorGUILayout.ObjectField("Spawn Point: ", spawnPoint.objectReferenceValue, typeof(Transform), true);

            // serializedObject only exists in the inspector, so ApplyModifiedProperties
            //  is called in order to save all these variables to the original script/instance
            serializedObject.ApplyModifiedProperties();

            //--
            //------ Serialized Property ------//



            //------ Wave system using targets ------//
            //
            // targets is a variable that directly references the original script/instance
            // we can directly change the variables inside targets as long as we use a cast.
            // For example, we could easily use targets
            // First, I have to set the local waves variable to match the one
            if (waves == null)
                waves = (target as WaveSpawning).waves;

            // 10 pixel gap etween horizontal lines
            GUILayout.Space(10);

            GUILayout.Label("Waves");

            //Screen.width gives us the width of the current window/tab in pixels
            // using something like "width * 0.1f" lets us set something to be
            // 10% the size of the window
            float width = Screen.width;

            //Without BeginHorizontal, each field would be its own line
            // this lets us place all the labels into one horizontal line
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(10) });

            // Button to add a new wave
            if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width(width * 0.05f) }))
            {
                waves.Add(new WaveSpawning.Wave());
            }

            //A label field is just plain text
            EditorGUILayout.LabelField("", new GUILayoutOption[] { GUILayout.Width(width * 0.05f) });
            //GUILayoutOption is an optional parameter that allows us to specify certain details like width

            //We can use new GUIContent for optional extras like icons or tooltips
            EditorGUILayout.LabelField(new GUIContent("Enemy", Resources.Load("Logos/AIE") as Texture, "What enemy to spawn"),
                new GUILayoutOption[] { GUILayout.Width(width * 0.3f) });
            EditorGUILayout.LabelField("Count",
                new GUILayoutOption[] { GUILayout.Width(width * 0.15f) });
            EditorGUILayout.LabelField("Seperation",
                new GUILayoutOption[] { GUILayout.Width(width * 0.15f) });
            EditorGUILayout.LabelField("Cooldown",
                new GUILayoutOption[] { GUILayout.Width(width * 0.15f) });

            //Make sure to end each horizontal section
            EditorGUILayout.EndHorizontal();

            //This renders each wave as a horizontal line.
            foreach (WaveSpawning.Wave wave in waves)
            {
                if (!WaveLine(wave))
                {
                    break;
                }
            }

            //Because I've only been setting the local waves,
            // I need to set the waves inside the instance as well
            (target as WaveSpawning).waves = waves;

            //
            //------ Wave system using targets ------//

        }

        /// <summary>
        /// Render a single wave as a horizontal line
        /// </summary>
        /// <param name="wave"></param>
        /// <returns></returns>
        bool WaveLine(WaveSpawning.Wave wave)
        {
            // Screen.width gives us the width of the current window/tab in pixels
            //  using something like "width * 0.1f" lets us set something to be
            //  10% the size of the window
            float width = Screen.width;

            // Without BeginHorizontal, each field would be its own line
            //  this lets us place all the labels into one horizontal line
            EditorGUILayout.BeginHorizontal();

            //A label field is just plain text
            EditorGUILayout.LabelField("", new GUILayoutOption[] { GUILayout.Width(width * 0.1f) });

            //GUILayoutOption is an optional parameter that allows us to specify certain details like width
            wave.enemy = (EditorGUILayout.ObjectField(wave.enemy, typeof(GameObject), true,
                new GUILayoutOption[] { GUILayout.Width(width * 0.3f) }) as GameObject);
            wave.count = EditorGUILayout.IntField(wave.count,
                new GUILayoutOption[] { GUILayout.Width(width * 0.15f) });
            wave.seperation = EditorGUILayout.FloatField(wave.seperation,
                new GUILayoutOption[] { GUILayout.Width(width * 0.15f) });
            wave.cooldown = EditorGUILayout.FloatField(wave.cooldown,
                new GUILayoutOption[] { GUILayout.Width(width * 0.15f) });

            if (GUILayout.Button("X"))
            {
                waves.Remove(wave);
                EditorGUILayout.EndHorizontal();
                return false;
            }

            EditorGUILayout.EndHorizontal();
            return true;
        }
    }
}