using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MassDisabler : EditorWindow
{
    private enum TypeToTarget
    {
        collider,
        renderer,
        shadowRendering,
    }

    private TypeToTarget typeToTarget = TypeToTarget.collider;
    private bool isSetToEnable = false;

    [MenuItem("Tools/Open Mass Disabler")]
    public static void ShowWindow()
    { 
        GetWindow<MassDisabler>("Mass Disabler");
    }

    private void OnGUI()
    {
        typeToTarget = (TypeToTarget)EditorGUILayout.EnumPopup(typeToTarget);
        isSetToEnable = EditorGUILayout.Toggle(isSetToEnable);


        if (GUILayout.Button(GetTitle()))
            DoIt(typeToTarget, isSetToEnable);
    }

    string GetTitle()
    {
        string str = string.Empty;
        str += isSetToEnable ? "Enable " : "Disable ";

        switch (typeToTarget)
        {
            case TypeToTarget.collider:
                str += "Colliders";
                break;
            case TypeToTarget.renderer:
                str += "Renderers";
                break;
            case TypeToTarget.shadowRendering:
                str += "Shadows";
                break;
            default:
                str += "???";
                break;
        }
        return str;
    }
    private void DoIt(TypeToTarget type, bool enable)
    {
        EditorGUI.BeginChangeCheck();

        switch (type)
        {
            case TypeToTarget.collider:
                ModifyColliders(Selection.gameObjects, enable);
                break; 
            case TypeToTarget.renderer:
                ModifyRenderers(GetAll<Renderer>(Selection.gameObjects), enable);
                break;
            case TypeToTarget.shadowRendering:
                ModifyShadows(GetAll<Renderer>(Selection.gameObjects), enable);
                break;
            default:
                Debug.LogWarning($"Type '{type}' hasnt been implimented");
                break;
        }

        //var objs = new List<GameObject>();
        //
        //for (int i = 0; i < targets.Length; i++)
        //{
        //    if (targets[i] == null)
        //        return;
        //
        //    
        //    DestroyImmediate(targets[i]);
        //}

        EditorGUI.EndChangeCheck();
    }
    private void ModifyColliders(GameObject[] selection, bool enable, bool remove = false)
    {
        var cols = new List<Collider>();

        foreach (GameObject go in selection)
            cols.AddRange(go.GetComponentsInChildren<Collider>());

        for (int i = 0; i < cols.Count; i++)
        {
            cols[i].enabled = enable;
            if (remove)
                DestroyImmediate(cols[i]);
        }
    }
    private List<T> GetAll<T>(GameObject[] selection) where T : Component
    {
        var objs = new List<T>();

        foreach (GameObject go in selection)
            objs.AddRange(go.GetComponentsInChildren<T>());

        return objs;
    }
    private void ModifyRenderers(List<Renderer> renderers, bool enable, bool remove = false)
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].enabled = enable;
            if (remove)
            {
                DestroyImmediate(renderers[i]);
                DestroyImmediate(renderers[i].GetComponent<MeshFilter>());

            }
        }
    }
    private void ModifyShadows(List<Renderer> renderers, bool enable)
    {
        foreach (Renderer r in renderers)
        {
            r.shadowCastingMode = enable ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    private void AddToList(GameObject root, ref List<GameObject> list)
    {
    }
}
