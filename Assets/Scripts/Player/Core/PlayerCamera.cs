using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] InputActionReference input_look;

        [SerializeField] Vector2 lookAngles;
        [SerializeField] Vector2 lookDelta;
        public Vector2 LookDelta => lookDelta;
        [Range(0, 25)]
        [SerializeField] float speed = 1;

        [SerializeField] bool canLook;

        [SerializeField] Transform playerBody;
        [SerializeField] Transform playerCamera;

        void Start()
        {
            LockControls(false);
        }

        void Update()
        {
            if (canLook)
                UpdateLook();
        }

        public void UpdateLook()
        {
            lookDelta = input_look.action.ReadValue<Vector2>() * speed * Time.deltaTime;
            lookAngles += lookDelta;
            lookAngles.y = Mathf.Clamp(lookAngles.y, -90, 90);

            playerCamera.localEulerAngles = new Vector3(-lookAngles.y, 0, 0);
            playerBody.localEulerAngles = new Vector3(0, lookAngles.x, 0);
        }
        public void LockControls(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isLocked;
            canLook = !isLocked;
        }

        public void SetSensitivity(float v) => speed = v;
        public float GetSensitivity() => speed;
    }
}