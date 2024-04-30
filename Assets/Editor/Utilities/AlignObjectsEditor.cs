using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AlignObjectsEditor : Editor
{
    [MenuItem("Tools/Quick Align")]
    public static void DoIt()
    {
        float rotationThresh = 15;
        float posThresh = .5f;
        float scaleThresh = .5f;

        foreach (var obj in Selection.gameObjects)
        {
            var tran = obj.transform;
            if (tran == null)
                continue;

            tran.localPosition = roundVector(tran.localPosition, posThresh);
            tran.localEulerAngles = roundVector(tran.localEulerAngles, rotationThresh);
            tran.localScale = roundVector(tran.localScale, scaleThresh);
        }
        Vector3 roundVector(Vector3 value, float threashold)
        {
            value.x = round(value.x, threashold);
            value.y = round(value.y, threashold);
            value.z = round(value.z, threashold);
            return value;
        }
        float round(float value, float threshold)
        {
            return Mathf.Round(value / threshold) * threshold;
        }
    }
}
