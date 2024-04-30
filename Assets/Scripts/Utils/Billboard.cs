using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam;

    public bool verticalOnly;

    void Start()
    {
        cam = Camera.main;

        if (cam == null )
        {
            Debug.LogWarning("There are no main cameras in the scene");
            this.enabled = false;
        }    
    }

    void LateUpdate()
    {
        if (!verticalOnly)
            transform.LookAt(cam.transform.position, Vector3.up);
        else
        {
            var dir = (cam.transform.position.ZeroY() - transform.position.ZeroY()).normalized;
            var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, angle, transform.localEulerAngles.z);
        }


    }
}
