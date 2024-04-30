using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerCameraSway : MonoBehaviour
    {
        public Transform playerHands;

        public new PlayerCamera camera;
        public PlayerMovement movement;

        public Vector2 rotationSmoothing = Vector2.one * 10;
        public Vector2 rotationScale = Vector2.one;
        public Vector2 rotationClamp;

        public AnimationCurve horzSwayCurve;
        public AnimationCurve vertSwayCurve;

        public Vector2 velocityThreshold;
        public Vector2 velocityMulti;

        //public float velocityThreshold;
        //public Vector3 velocitySmoothing = Vector3.one;
        //public Vector3 veloctiyScale = Vector3.one;

        //add jumping and landing movement
        //add effecting rotation with horizontal camera movement

        void Start()
        {

        }

        void Update()
        {
            {
                float vertVel = 0;
                if (Mathf.Abs(movement.velocity.y) >= velocityThreshold.y)
                    vertVel = movement.velocity.y * velocityMulti.y;

                float horzVel = 0;
                if (Mathf.Abs(movement.localVelocity.x) >= velocityThreshold.x)
                    horzVel = movement.localVelocity.x * velocityMulti.x;

                var baseAngles = playerHands.localEulerAngles;
                var ld = camera.LookDelta;

                var x = Mathf.Clamp(ld.y * rotationScale.y + vertVel, -rotationClamp.x, rotationClamp.x);
                var y = Mathf.Clamp(ld.x * rotationScale.x + horzVel, -rotationClamp.y, rotationClamp.y);

                x = MapToCurve(x, rotationClamp.x, vertSwayCurve);
                y = MapToCurve(y, rotationClamp.y, horzSwayCurve);

                var goal = new Vector3(x, y);
                //goal = Vector2.Scale(goal, rotationScale);

                float vertical = Mathf.LerpAngle(baseAngles.x, goal.x, rotationSmoothing.x * Time.deltaTime);
                float horizontal = Mathf.LerpAngle(baseAngles.y, goal.y, rotationSmoothing.y * Time.deltaTime);

                playerHands.localEulerAngles = new Vector3(vertical, horizontal, 0);
                //.Limit(rotationClamp.x, rotationClamp.y, 0);
            }
            /*{
                var basePos = playerHands.localPosition;
                var vel = movement.velocity;
                vel = movement.transform.InverseTransformVector(vel);

                Vector3 goal = vel.magnitude > velocityThreshold
                    ? Vector3.Scale(vel.normalized, -veloctiyScale)
                    : Vector3.zero;

                float x = Mathf.Lerp(basePos.x, goal.x, velocitySmoothing.x * Time.deltaTime);
                float y = Mathf.Lerp(basePos.y, goal.y, velocitySmoothing.y * Time.deltaTime);
                float z = Mathf.Lerp(basePos.z, goal.z, velocitySmoothing.z * Time.deltaTime);

                playerHands.localPosition = new Vector3(x, y, z);
            }*/
        }
        float MapToCurve(float value, float max, AnimationCurve curve)
        {
            float sign = Mathf.Sign(value);

            float normalizedValue = Mathf.Clamp01(Mathf.Abs(value) / max);

            float mappedValue = curve.Evaluate(normalizedValue) * max;

            return mappedValue * sign;
        }
    }
}