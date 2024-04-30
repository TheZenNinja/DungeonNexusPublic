using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hurtbox : MonoBehaviour
{
    public UnityEvent<Health> onHit;

    public LayerMask ignoreMask;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (TryGetComponent(out Health health))
        {
            if (ClassExtensions.LayerIsInMask(other.gameObject.layer, ~ignoreMask))
            { 
                onHit?.Invoke(health);
            }
        }
    }
}
