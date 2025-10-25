using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerDetector : MonoBehaviour
{
        public static event Action<int, bool> OnInputChange;

        private string[] _joystickNames;

        private void OnEnable() {
                InputSystem.onDeviceChange += OnDeviceChange;
        }

        private void OnDisable() {
                InputSystem.onDeviceChange -= OnDeviceChange;
        }

        void Start() {
                _joystickNames = Input.GetJoystickNames();

                if (_joystickNames.Length == 1) {
                        Debug.Log("No gamepad is connected.");
                        OnInputChange?.Invoke(-1, false);
                } else if (_joystickNames.Length == 2) {
                        Debug.Log("player one is connected by controller");
                        OnInputChange?.Invoke(1, true);
                } else {
                        Debug.Log("player two is connected by controller");
                        OnInputChange?.Invoke(2, true);
                }
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change) {
                if (device is Gamepad gamepad) {
                        _joystickNames = Input.GetJoystickNames();

                        switch (change) {
                                case InputDeviceChange.Added:
                                        Debug.Log($"Gamepad '{gamepad.displayName}' connected!");
                                        if (_joystickNames.Length == 1) {                                            
                                                OnInputChange?.Invoke(1, true);                                        
                                        } else {
                                                OnInputChange?.Invoke(2, true);
                                        }
                                        break;

                                case InputDeviceChange.Removed:
                                        Debug.Log($"Gamepad '{gamepad.displayName}' disconnected!");
                                        if (_joystickNames.Length == 2) {
                                                OnInputChange?.Invoke(2, false);
                                        } else {
                                                OnInputChange?.Invoke(1, false);
                                        }
                                        break;
                        }
                }
        }
}
