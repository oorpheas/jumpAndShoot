using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{

    public int speed;
    public int playerID;

    public float jumpForce;  

    static public int pID;
    static public bool isFlipped;
    static public bool isFlipped2;

    [SerializeField]
    private GameObject bulletPrefab;

    private float _axisX;
    
    private bool _isGrounded;

    private Rigidbody2D _rb2d;

    private SpriteRenderer _sR;

    private Transform _gunL;
    private Transform _gunR;
    private Transform _armaUsada;

    private Animator _animator;

    private GameObject _spawn;

    // Start is called before the first frame update
    void Start()
    {
        pID = playerID;

        _animator = GetComponent<Animator>();
        
        if (playerID == 1)
        {
            _spawn = GameObject.FindGameObjectWithTag("playerSpawn");
            gameObject.transform.position = _spawn.transform.position;
            _gunL = GetComponentInChildren<Transform>().Find("gunL");
            _gunR = GetComponentInChildren<Transform>().Find("gunR");
        }
        else if (playerID == 2)
        {
            _spawn = GameObject.FindGameObjectWithTag("playerSpawn2");

            //_anim = gameObject.GetComponent<Animation>();
            gameObject.transform.position = _spawn.transform.position;
        }
        else
        {
            Debug.LogWarning("não tem controles associados a esse ID");
        }

        _rb2d = GetComponent<Rigidbody2D>();
        _sR = GetComponent<SpriteRenderer>();

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

    void SetFlip()
    {
        _animator.SetFloat("axisX", _axisX);

        //if (playerID == 1)
        //{
        //    isFlipped = _sR.flipX;
        //}
        //else if (playerID == 2)
        //{
        //    isFlipped2 = _sR.flipX;
        //}
    }

    void SetMoviment()
    {
        if (playerID == 1)
        {
            _axisX = Input.GetAxis("Horizontal-P1");
            Moviment(_axisX);
            _animator.SetBool("isWalking", true);
        }
        else if (playerID == 2)
        {
            _axisX = Input.GetAxis("Horizontal-P2");
            Moviment(_axisX);
            _animator.SetBool("isWalking", true);
        }
        else
        {
            Debug.LogWarning("não tem controles associados a esse ID");
        }
    }

    void Moviment(float axis)
    {
        _rb2d.velocity = new Vector2(axis * speed, _rb2d.velocity.y); // em Y ele está recebendo a velocidade atribuida (ou seja, gravidade);

        if (axis < 0)
        {
            _armaUsada = _gunL.transform;
            isFlipped = true;
        }
        else if (axis > 0)
        {
            _armaUsada = _gunR.transform;
            isFlipped = false;
        }

        SetFlip();
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
                SetShoot(_armaUsada, isFlipped);
            }
        }
        else if (playerID == 2)
        {
            if (Input.GetKeyUp(KeyCode.RightControl))
            {
                SetShoot(_armaUsada, isFlipped);
            }
        }
        else
        {
            Debug.LogWarning("não tem controles associados a esse ID");
        }
    }

    //void Knife()
    //{
    //    if (playerID == 1)
    //    {
    //        if (Input.GetKeyDown(KeyCode.LeftControl))
    //        {
    //            SetShoot(_armaUsada, isFlipped);
    //        }
    //    }
    //    else if (playerID == 2)
    //    {
    //        if (!_anim.isPlaying)
    //        {
    //            if (Input.GetKeyUp(KeyCode.RightControl))
    //            {
    //                _anim.Play("attack");
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("não tem controles associados a esse ID");
    //    }
    //}

    void SetShoot(Transform _pos, bool _flip)
    {
        if (!_flip)
        {
            Instantiate(bulletPrefab, _pos.transform.position, transform.rotation);
        }
        else
        {
            Instantiate(bulletPrefab, _pos.transform.position, new Quaternion(0, -180, 0, 0));
        }

        _animator.SetBool("isShooting", true);
                

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
