using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("Especificações")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private int _ammoCapacity;

    private float _timer;

    private bool _isGrounded, _isWalking;

    private static Rigidbody2D _rb2d;

    private Transform _gunL, _gunR, _armaUsada;

    public static float axis;


    // Start is called before the first frame update
    void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _gunL = GetComponentInChildren<Transform>().Find("gunL");
        _gunR = GetComponentInChildren<Transform>().Find("gunR");
        _armaUsada = _gunR.transform;

    }

    private void Moved()
    {
        _rb2d.velocity = new Vector2(axis * _speed, _rb2d.velocity.y);
    }
}
