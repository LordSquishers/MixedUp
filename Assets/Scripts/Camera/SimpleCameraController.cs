using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityTemplateProjects
{
    public class SimpleCameraController : MonoBehaviour
    {

        SimpleCameraControls controls;
        public Vector2 Move = new Vector2();
        public Action<Vector2> OnMove;

        class CameraState
        {
            public float yaw;
            public float pitch;
            public float roll;
            public float x;
            public float y;
            public float z;

            public void SetFromTransform(Transform t)
            {
                pitch = t.eulerAngles.x;
                yaw = t.eulerAngles.y;
                roll = t.eulerAngles.z;
                x = t.position.x;
                y = t.position.y;
                z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

                x += rotatedTranslation.x;
                y += rotatedTranslation.y;
                z += rotatedTranslation.z;
            }

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
                roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);
                
                x = Mathf.Lerp(x, target.x, positionLerpPct);
                y = Mathf.Lerp(y, target.y, positionLerpPct);
                z = Mathf.Lerp(z, target.z, positionLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(pitch, yaw, roll);
                t.position = new Vector3(x, y, z);
            }
        }
        
        CameraState m_TargetCameraState = new CameraState();
        CameraState m_InterpolatingCameraState = new CameraState();

        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        public float boost = 3.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        public float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        public bool invertY = false;

        void OnEnable()
        {
            controls = new SimpleCameraControls();
            controls.Player.Move.performed += context => OnMovePerformed(context); // Bind a function to the move performed event.
            controls.Player.Move.canceled += context => OnMoveCanceled(context); // Bind a function to the move canceled event.

            controls.Enable(); // Dont forget to enable the inputs. It wont work otherwise.
            m_TargetCameraState.SetFromTransform(transform);
            m_InterpolatingCameraState.SetFromTransform(transform);
        }

        void OnDisable() {
            controls.Disable();
        }

        private void OnMovePerformed(InputAction.CallbackContext context) {
            // We check if OnMove has functions bind to that event and if yes, we will fire them and input the Vector2 value delivered by our context
            if (OnMove != null) {
                OnMove(context.ReadValue<Vector2>()); // context.ReadValue<Vector2>() is how you read the value from the input. Context got passed through in our OnEnable event binding before.
            }
        }

        private void OnMoveCanceled(InputAction.CallbackContext context) {
            // We check if OnMove has functions bind to that event and if yes, we will fire them and input the Vector2 value delivered by our context
            if (OnMove != null) {
                OnMove(context.ReadValue<Vector2>()); // context.ReadValue<Vector2>() is how you read the value from the input. Context got passed through in our OnEnable event binding before.
            }
        }

        Vector3 GetInputTranslationDirection()
        {
            var keyboard = Keyboard.current;
            Vector3 direction = new Vector3();
            if (keyboard[Key.W].isPressed)
            {
                direction += Vector3.forward;
            }
            if (keyboard[Key.S].isPressed)
            {
                direction += Vector3.back;
            }
            if (keyboard[Key.A].isPressed)
            {
                direction += Vector3.left;
            }
            if (keyboard[Key.D].isPressed)
            {
                direction += Vector3.right;
            }
            if (keyboard[Key.Q].isPressed)
            {
                direction += Vector3.down;
            }
            if (keyboard[Key.E].isPressed)
            {
                direction += Vector3.up;
            }
            return direction;
        }
        
        void Update()
        {
            Vector3 translation = Vector3.zero;

            // Exit Sample  
            if (Keyboard.current[Key.Escape].isPressed)
            {
                Application.Quit();
				#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false; 
				#endif
            }
            // Hide and lock cursor when right mouse button pressed
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
            }

            // Unlock and show cursor when right mouse button released
            if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            // Rotation
            if (Mouse.current.rightButton.isPressed)
            {
                Move = controls.Player.Move.ReadValue<Vector2>();
                var mouseMovement = new Vector2(Move.x, Move.y * -1)* 0.05f; // Vector 2!
                
                var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            }
            
            // Translation
            translation = GetInputTranslationDirection() * Time.deltaTime * 2;

            // Speed up movement when shift key held
            if (Keyboard.current[Key.LeftShift].isPressed)
            {
                translation *= 2.5f;
            }

            m_TargetCameraState.Translate(translation);

            // Framerate-independent interpolation
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

            m_InterpolatingCameraState.UpdateTransform(transform);
        }
    }

}