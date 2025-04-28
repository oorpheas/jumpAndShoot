using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static PlayerInput;

public class PlayerInput : MonoBehaviour
{
    public ControllerSelect controllerSelector;

    public enum ControllerSelect
    {
        PlayerOne,
        PlayerTwo
    }

    public event Action Move, Jump, Shoot, Reload, Attack;

    private KeyCode _jumpKey, _shootKey, _reloadKey, _attackKey;
    private float _moveDirection;

    public void InputManager()
    {   
        bool playerOne = (ControllerSelect.PlayerOne == 0) ? true : false;

        if (playerOne) {
            PInput("Horizontal-P1", KeyCode.W, KeyCode.LeftControl, KeyCode.R, KeyCode.RightShift);
        } else {
            PInput("Horizontal-P2", KeyCode.UpArrow, KeyCode.RightControl, KeyCode.Return, KeyCode.LeftShift);
        }
        
        

        if (_moveDirection != 0) {
            //Move?.Invoke(moveDirection);
        }
    }
    public void PInput(string mName, KeyCode jKey, KeyCode sKey, KeyCode rKey, KeyCode aKey)
    {
        _moveDirection = Input.GetAxis(mName);
        _jumpKey = jKey;
        _shootKey = sKey;
        _reloadKey = rKey;
        _attackKey = aKey;
    }
}
