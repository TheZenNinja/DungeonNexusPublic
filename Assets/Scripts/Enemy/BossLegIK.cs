using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLegIK : MonoBehaviour
{
    [System.Serializable]
    public class LegData
    {
        public Transform goalObj;
        public Transform jointBase;
        public Transform defaultPos;
        public Transform raycastOrigin;
        [Space]
        public bool isMoving = false;
        public Vector3 oldPos;
        public Vector3 raycastPos;
    }

    public float legRaycastDistance = 10;
    public float maxLegSpace = 4;
    public float legMoveDuration = .2f;
    public float spherecastRadius = .2f;
    public LayerMask terrainLayer;

    public LegData[] legs;

    
    private void FixedUpdate()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            var leg = legs[i];

            // Getting raycast
            var ray = new Ray(leg.raycastOrigin.position, leg.raycastOrigin.forward);
            //if (Physics.Raycast(ray, out RaycastHit hit, legRaycastDistance, terrainLayer))
            if (Physics.SphereCast(ray, spherecastRadius, out RaycastHit hit, legRaycastDistance, terrainLayer))
            {
                leg.raycastPos = hit.point;

                if (Vector3.Distance(leg.oldPos, leg.raycastPos) >= maxLegSpace)
                    if (CanMoveLegs() && !leg.isMoving)
                        StartCoroutine(MoveRoutine(legMoveDuration, leg.raycastPos));
            }
            else
            {
                leg.oldPos = ray.GetPoint(legRaycastDistance/2); //leg.defaultPos.position;
            }

            leg.goalObj.forward = leg.raycastOrigin.forward;
            leg.goalObj.position = leg.oldPos;

            IEnumerator MoveRoutine(float duration, Vector3 goal)
            { 
                leg.isMoving = true;
                DOTween.To(() => leg.oldPos, x => leg.oldPos = x, goal, duration);
                yield return new WaitForSeconds(duration);
                leg.isMoving = false;
                leg.oldPos = goal;
            }
        }
    }
    /*private void FixedUpdate()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            var leg = legs[i];
            CheckPosition(maxLegSpace, terrainLayer);

            leg.goalObj.position = leg.oldPos;

            void CheckPosition(float maxDistToGoal, LayerMask terrainLayer)
            {
                RaycastTarget(terrainLayer);

                if (Vector3.Distance(leg.oldPos, leg.raycastPos) >= maxDistToGoal)
                    UpdatePosition();
            }

            void UpdatePosition()
            {
                if (leg.isMoving)
                    return;
                if (!CanMoveLegs())
                    return;

                //DOTween.To(() => leg.oldPos, x => leg.oldPos = x, leg.currentPos, .4f);

                StartCoroutine(MoveRoutine(.1f, leg.raycastPos));
            }
            IEnumerator MoveRoutine(float delay, Vector3 goal)
            { 
                leg.isMoving = true;
                //leg.goalObj.DOMove(goal, delay);
                DOTween.To(() => leg.oldPos, x => leg.oldPos = x, goal, delay);

                yield return new WaitForSeconds(delay);

                leg.isMoving = false;
                leg.oldPos = goal;
            }

            void RaycastTarget(LayerMask terrainLayer)
            {
                var ray = new Ray(leg.jointBase.position, leg.jointBase.forward);

                if (Physics.Raycast(ray, out RaycastHit hit, legRaycastDistance, terrainLayer))
                    leg.raycastPos = hit.point;
                else
                    leg.raycastPos = leg.defaultPos.position;

            }
        }
    }*/
    private bool CanMoveLegs()
    {
        int maxMovableLegs = 3;
        foreach (var leg in legs)
        {
            if (leg.isMoving)
                maxMovableLegs--;
            if (maxMovableLegs <= 0)
                return false;
        }
        return true;
    }
    private void OnDrawGizmos()
    {
        foreach (var leg in legs)
        {
            Gizmos.color = Color.yellow;
            if (leg.raycastOrigin)
            Gizmos.DrawLine(leg.raycastOrigin.position, leg.raycastOrigin.position + leg.raycastOrigin.forward * legRaycastDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(leg.oldPos, .1f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(leg.raycastPos, .1f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(leg.defaultPos.position, .1f);
        }
    }
}
