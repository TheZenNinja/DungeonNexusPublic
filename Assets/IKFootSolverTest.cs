using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolverTest : MonoBehaviour
{
    public Transform body;
    public float heightOffset;
    public float lookAheadDist = .6f;
    public float footSpacing;
    public LayerMask terrainLayer;

    public AnimationCurve stepCurve;

    public float stepLength = 1;

    public float stepSpeed = 1;
    [Range(0f, 1f)]
    public float stepPercent;
    public float stepHeight = 1;
    public Vector3 oldPos, currentPos, goalPos;
    
    // unite both legs in one script
    // instead of using lookAhead, use the direction between old and goal pos
    // have separate step spacing variables based on how fast the entity is moving
    // -- blend between the two values? (walking vs sprinting)
    void Start()
    {
        stepPercent = 1;
        currentPos = goalPos = oldPos = transform.position;
    }

    void Update()
    {
        transform.position = currentPos;

        var raycastPos = body.position +
            body.forward * lookAheadDist +
            (body.right * footSpacing) +
            Vector3.up * heightOffset;

        Ray ray = new Ray(raycastPos, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer))
        {
            if (VectorExtensions.DistanceXZ(goalPos, info.point) >= stepLength)
            {
                goalPos = info.point;
                stepPercent = 0;
            }    
        }

        if (stepPercent < 1)
        {
            var perc = stepCurve.Evaluate(stepPercent);
            Vector3 pos = Vector3.Lerp(oldPos, goalPos, perc);
            pos.y = Mathf.Sin(perc * Mathf.PI) * stepHeight;

            currentPos = pos;

            stepPercent += Time.deltaTime * stepSpeed;

            if (stepPercent > 1)
                stepPercent = 1;
        }
        else
        {
            oldPos = currentPos;
        }
    }
    private void OnDrawGizmos()
    {
        if (body != null)
        {
            Gizmos.color = Color.red;
            var startPos = body.position + (body.right * footSpacing) + Vector3.up * heightOffset;
            Gizmos.DrawLine(startPos, startPos + Vector3.down * 5);
        }

        Gizmos.color = Color.red;
        var dir = (goalPos - oldPos).normalized * stepLength;
        Gizmos.DrawLine(oldPos, oldPos + dir);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(oldPos, .1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(currentPos, .1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(goalPos, .1f);
    }
}
