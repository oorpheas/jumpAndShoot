using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{

    public int speed;
    public float jumpForce;

    static public bool isFlipped;

    [SerializeField]
    private GameObject bulletPrefab;
    private float _axisX;

    private Rigidbody2D _rb2d;
    private bool _isGrounded;
    private SpriteRenderer _sR;

    private Transform _gunL;
    private Transform _gunR;
    private Transform _armaUsada;

    private GameObject _spawn;

    // Start is called before the first frame update
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _sR = GetComponent<SpriteRenderer>();
        _spawn = GameObject.FindGameObjectWithTag("playerSpawn");

        gameObject.transform.position = _spawn.transform.position;

        _gunL = GetComponentInChildren<Transform>().Find("gunL");
        _gunR = GetComponentInChildren<Transform>().Find("gunR");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Jump();
        Moviment();
    }

    private void Update()
    {
        Shoot();   
    }

    void Moviment()
    {
        _axisX = Input.GetAxis("Horizontal");
        _rb2d.velocity = new Vector2 (_axisX * speed, _rb2d.velocity.y); // em Y ele está recebendo a velocidade atribuida (ou seja, gravidade);

        if (_axisX < 0)
        {
            _sR.flipX = true;
            _armaUsada = _gunL.transform;
            isFlipped = true;
        }
        else if (_axisX > 0)
        {
            _sR.flipX = false;
            _armaUsada = _gunR.transform;
            isFlipped = false;
        }

    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.W) && _isGrounded || Input.GetKey(KeyCode.UpArrow) && _isGrounded) // qualquer variavel bool é entendida como TRUE dentro de verificações, exceto se for !bool
        {
            Vector2 _jump = new Vector2(0f, jumpForce);
            _rb2d.AddForce(_jump, ForceMode2D.Impulse);
        }
    }

    void Shoot()
    { 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(bulletPrefab, _armaUsada.transform.position, transform.rotation);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ground"))
        {
            _isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ground"))
        {
            _isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            Destroy(gameObject);
        }       
    }
}
