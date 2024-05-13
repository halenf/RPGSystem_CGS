using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace AdamAssets
{
    public class MYWindow : EditorWindow
    {
        float x;

        //This is require to add a menu option for this window
        [MenuItem("Window/My Window")]
        public static void ShowWindow()
        {
            GetWindow(typeof(MYWindow));
        }

        void OnGUI()
        {
            //A field to save a float
            x = EditorGUILayout.FloatField("My Float", x);

            //A button to spawn a cube
            if (GUILayout.Button("My Button"))
            {
                Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube),
                    new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)), Quaternion.identity);
            }

            //Draw a box with a texture inside it
            GUILayout.Box(Resources.Load("Logos/AIE") as Texture);
        }
    }
}