using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        // TODO: dashing, sliding, doublejumping, enemy-step, jump equation, jump height cutting

        public float speed;
        public float accel;

        public float gravity;
        public float jumpHeight;
        [SerializeField] float shortHopBoost;
        [Range(0f, 1f)]
        [Tooltip("The hight of the jump where shorthopping is still valid (1=apex)")]
        [SerializeField] float shorthopArcThresh;

        public Vector3 velocity;
        public Vector3 localVelocity => transform.InverseTransformVector(velocity);
        CharacterController cc;


        public bool areControlsLocked;

        Entity entity;

        public InputActionReference input_movement;
        public InputActionReference input_jump;

        void Start()
        {
            LockControls(false);
            cc = GetComponent<CharacterController>();
            entity = GetComponent<Entity>();
            input_jump.action.canceled += (_) => CheckShorthop();
        }

        private void CheckShorthop()
        {
            if (velocity.y <= (1 - shorthopArcThresh) * GetJumpVelocity())
                return;

            velocity.y *= .5f;
            velocity += GetInputVector() * speed * shortHopBoost;
        }

        void Update()
        {
            if (!areControlsLocked && input_jump.action.triggered && cc.isGrounded)
                Jump();

            cc.Move(velocity * Time.deltaTime);
        }

        void FixedUpdate()
        {
            Vector3 moveInput = Vector3.zero;
            if (!entity.IsMovementLocked)
                moveInput = GetInputVector() * accel;

            velocity = velocity.LerpXY(moveInput, accel * Time.fixedDeltaTime);

            //velocity = velocity.LerpXY(transform.TransformDirection(input_movement.action.ReadValue<Vector2>().normalized.ToV3() * accel), accel * Time.fixedDeltaTime);

            if (!cc.isGrounded)
            {
                velocity -= Vector3.up * gravity * Time.fixedDeltaTime;
            }
            else
            {
                if (!input_jump.action.IsPressed())
                    velocity.y = -0.01f;
            }

            //cc.Move(velocity * Time.fixedDeltaTime);
        }
        /*public void Dash(float distance, float duration, float invulnDuration)
        {
            Vector3 dir = transform.TransformVector(GetInputVector());
            int iterations = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
            Vector3 vel = dir.normalized * distance / duration;

            StartCoroutine(DashRoutine());
            IEnumerator DashRoutine()
            {
                e.SetLockedMove(true);
                for (int i = 0; i < iterations; i++)
                {
                    cc.Move(vel * Time.fixedDeltaTime);
                    yield return new WaitForFixedUpdate();
                }
                e.SetLockedMove(false);
            }
            StartCoroutine(Invuln());
            IEnumerator Invuln()
            {
                //have specific invuln for dashing and parry?
                e.Health.SetInvulnerable(true);
                yield return new WaitForSeconds(invulnDuration);
                e.Health.SetInvulnerable(false);
            }
        }*/
        public Vector3 GetInputVector() => transform.TransformDirection(input_movement.action.ReadValue<Vector2>().normalized.ToV3());

        public void Jump()
        {
            //taken from projectile formula
            velocity.y = GetJumpVelocity();
        }
        private float GetJumpVelocity() => Mathf.Sqrt(2 * gravity * jumpHeight);

        public void LockControls(bool isLocked)
        {
            areControlsLocked = isLocked;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + localVelocity);
        }
    }
}