using UnityEngine;
using UnityEngine.InputSystem;

namespace VRTraining.Debugging
{
    /// <summary>
    /// Временный помощник для тестирования сценария мышью/клавиатурой без шлема.
    /// XR Device Simulator сам по себе двигает клавишами WASD только позицию
    /// "устройства" (имитация трекинга головы) в пределах небольшого объёма —
    /// это НЕ движение через Locomotion System, поэтому игрок физически не
    /// доходит до триггерных зон. Этот компонент двигает CharacterController
    /// напрямую по направлению камеры, чтобы можно было быстро проверить логику
    /// сценария. Перед релизом под реальный VR-шлем компонент можно отключить
    /// или удалить — на реальном железе локомоция идёт через штатный
    /// Dynamic/Continuous Move Provider на XR Origin.
    /// </summary>
    public class KeyboardTestLocomotion : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float moveSpeed = 3f;

        private void Update()
        {
            if (characterController == null || cameraTransform == null) return;
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 move = Vector3.zero;
            if (keyboard.wKey.isPressed) move += forward;
            if (keyboard.sKey.isPressed) move -= forward;
            if (keyboard.dKey.isPressed) move += right;
            if (keyboard.aKey.isPressed) move -= right;

            if (move.sqrMagnitude > 1f) move.Normalize();

            characterController.Move(move * moveSpeed * Time.deltaTime);

            if (!characterController.isGrounded)
                characterController.Move(Physics.gravity * Time.deltaTime);
        }
    }
}
