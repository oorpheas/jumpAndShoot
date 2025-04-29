using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.ComponentModel;

public class PlayerInput : MonoBehaviour
{
    //public ControllerSelect controllerSelector; // cria o seletor

    public enum ControllerSelect // cria o dropdown
    {
        PlayerOne,
        PlayerTwo
    }

    public event Action Moved, Jumped, Shooted, Reloaded, Attacked; // prepara os eventos

    private KeyCode _jumpKey, _shootKey, _reloadKey, _attackKey; // prepara as teclas sem definir elas;

    private float _moveDirection;

    public void InputManager()
    {   
        bool playerOne = (ControllerSelect.PlayerOne == 0) ? true : false;

        if (playerOne) {
            PInput("Horizontal-P1", KeyCode.W, KeyCode.LeftControl, KeyCode.R, KeyCode.RightShift); // dava para deixar isso mais organico para personalizar em outro lugar;
        } else {
            PInput("Horizontal-P2", KeyCode.UpArrow, KeyCode.RightControl, KeyCode.Return, KeyCode.LeftShift);
        }
        
        if (_moveDirection != 0) {
            PlayerController.axis = _moveDirection;
            Debug.Log("entrou");
            Moved?.Invoke();
        }

        if (Input.GetKeyDown(_jumpKey)) {
            Jumped?.Invoke();
        }

        if (Input.GetKey(_shootKey)) {
            Shooted?.Invoke();
        }

        if (Input.GetKey(_reloadKey)) {
            Reloaded?.Invoke();
        }

        if (Input.GetKey(_attackKey)) {
            Attacked?.Invoke();
        }

    }

    public void PInput(string mName, KeyCode jKey, KeyCode sKey, KeyCode rKey, KeyCode aKey) // aqui faz a associação das teclas
    {
        _moveDirection = (Input.GetAxis(mName));
        _jumpKey = jKey;
        _shootKey = sKey;
        _reloadKey = rKey;
        _attackKey = aKey;
    }

}
