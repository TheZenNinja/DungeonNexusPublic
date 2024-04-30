using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Skills.Player;

[CustomEditor(typeof(PlayerQuickMelee))]
public class PlayerQuickMeleeEditor : Editor
{
    private void OnSceneGUI()
    {
        PlayerQuickMelee pqm = target as PlayerQuickMelee;
        if (pqm == null)
            return;

        if (pqm.camera == null)
            return;

        Handles.color = Color.red;
        Handles.DrawWireCube(pqm.camera.transform.TransformPoint(pqm.GetHitboxOffset), pqm.GetHitboxSize);
    }
}
