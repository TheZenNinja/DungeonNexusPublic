using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RandomizeSelection : EditorWindow
{
    public GameObject[] prefabs = new GameObject[0];
    [MenuItem("Tools/Open Randomizer")]
    public static void ShowWindow()
    { 
        GetWindow<RandomizeSelection>("Randomizer");
    }

    private void OnGUI()
    {
        var serObj = new SerializedObject(this);
        var objsProp = serObj.FindProperty("prefabs");
        EditorGUILayout.PropertyField(objsProp, true);
        serObj.ApplyModifiedProperties();


        if (GUILayout.Button("Randomize") && prefabs.Length > 0)
            DoIt();
    }
    public void DoIt()
    {
        EditorGUI.BeginChangeCheck();

        var targets = Selection.gameObjects;
        var newObjs = new GameObject[targets.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null)
                return;

            var newObj = Instantiate(GetRandomObj());
            newObj.transform.parent = targets[i].transform.parent;
            newObj.transform.SetPositionAndRotation(targets[i].transform.position, targets[i].transform.rotation);
            newObj.transform.localScale = newObj.transform.localScale;
            
            DestroyImmediate(targets[i]);

            newObjs[i] = newObj;
        }

        Selection.objects = newObjs;

        EditorGUI.EndChangeCheck();
    }
    private GameObject GetRandomObj() => prefabs[Random.Range(0, prefabs.Length)];
}
