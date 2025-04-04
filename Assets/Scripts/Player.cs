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
    
    public int playerID;
    static public int ID;
    static public bool[] isFlipped;

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
        ID = playerID;

        if (playerID == 1)
        {
            _spawn = GameObject.FindGameObjectWithTag("playerSpawn");

            gameObject.transform.position = _spawn.transform.position;
        }
        else if (playerID == 2)
        {
            _spawn = GameObject.FindGameObjectWithTag("playerSpawn2");

            gameObject.transform.position = _spawn.transform.position;
        }
        else
        {
            Debug.LogWarning("não tem controles associados a esse ID");
        }

        _rb2d = GetComponent<Rigidbody2D>();
        _sR = GetComponent<SpriteRenderer>();

        _gunL = GetComponentInChildren<Transform>().Find("gunL");
        _gunR = GetComponentInChildren<Transform>().Find("gunR");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Jump();
        SetMoviment();
    }

    private void Update()
    {
        Shoot();   
    }

    void SetMoviment()
    {
        if (playerID == 1)
        {
            _axisX = Input.GetAxis("Horizontal-P1");
            Moviment(_axisX, playerID);
        }
        else if (playerID == 2)
        {
            _axisX = Input.GetAxis("Horizontal-P2");
            Moviment(_axisX, playerID);
        }
        else
        {
            Debug.LogWarning("não tem controles associados a esse ID");
        }
    }

    void Moviment(float axis, int ID)
    {
        _rb2d.velocity = new Vector2(axis * speed, _rb2d.velocity.y); // em Y ele está recebendo a velocidade atribuida (ou seja, gravidade);

        if (axis < 0)
        {
            _sR.flipX = true;
            _armaUsada = _gunL.transform;
            isFlipped[ID] = true;
        }
        else if (axis > 0)
        {
            _sR.flipX = false;
            _armaUsada = _gunR.transform;
            isFlipped[ID] = false;
        }
    }

    void Jump()
    {
        if (playerID == 1)
        {
            if (Input.GetKey(KeyCode.W) && _isGrounded) // qualquer variavel bool é entendida como TRUE dentro de verificações, exceto se for !bool
            {
                Vector2 _jump = new Vector2(0f, jumpForce);
                _rb2d.AddForce(_jump, ForceMode2D.Impulse);
            }
        }
        else if (playerID == 2)
        {
            if (Input.GetKey(KeyCode.UpArrow) && _isGrounded) // qualquer variavel bool é entendida como TRUE dentro de verificações, exceto se for !bool
            {
                Vector2 _jump = new Vector2(0f, jumpForce);
                _rb2d.AddForce(_jump, ForceMode2D.Impulse);
            }
        }
        else
        {
            Debug.LogWarning("não tem controles associados a esse ID");
        }
    }

    void Shoot()
    {
        if (playerID == 1)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Instantiate(bulletPrefab, _armaUsada.transform.position, transform.rotation);
            }
        }
        else if (playerID == 2)
        {
            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                Instantiate(bulletPrefab, _armaUsada.transform.position, transform.rotation);
            }
        }
        else
        {
            Debug.LogWarning("não tem controles associados a esse ID");
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
