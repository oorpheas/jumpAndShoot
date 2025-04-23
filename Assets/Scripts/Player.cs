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
    
    private bool _isGrounded, _isWalking;

    private Rigidbody2D _rb2d;

    private SpriteRenderer _sR;

    private Transform _gunL;
    private Transform _gunR;
    private Transform _armaUsada;

    private Animator _animator;

    private GameObject _spawn;

    private string _animName;

    // Start is called before the first frame update
    void Start()
    {
        pID = playerID;

        _animator = GetComponent<Animator>();

        _gunL = GetComponentInChildren<Transform>().Find("gunL");
        _gunR = GetComponentInChildren<Transform>().Find("gunR");
        
        if (playerID == 1)
        {
            _spawn = GameObject.FindGameObjectWithTag("playerSpawn");
            gameObject.transform.position = _spawn.transform.position;
        }
        else if (playerID == 2)
        {
            _spawn = GameObject.FindGameObjectWithTag("playerSpawn2");
            gameObject.transform.position = _spawn.transform.position;

            //_anim = gameObject.GetComponent<Animation>();
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
        _animator.SetBool("isFlipped", isFlipped);
    }

    void SetMoviment()
    {
        if (playerID == 1)
        {
            _axisX = Input.GetAxis("Horizontal-P1");
            Moviment(_axisX);
        }
        else if (playerID == 2)
        {
            _axisX = Input.GetAxis("Horizontal-P2");
            Moviment(_axisX);
        }
        else
        {
            Debug.LogWarning("não tem controles associados a esse ID");
        }

        _animator.SetBool("isWalking", _isWalking);
        _animator.SetBool("isIdle", !_isWalking);
        Debug.LogWarning("player está: \n idle = " + _animator.GetBool("isIdle") + " || andando = " + _animator.GetBool("isWalking") + " || tá virado? " + _animator.GetBool("isFlipped"));
    }

    void Moviment(float axis)
    {
        _rb2d.velocity = new Vector2(axis * speed, _rb2d.velocity.y); // em Y ele está recebendo a velocidade atribuida (ou seja, gravidade);

        if (_rb2d.velocity.x == 0f)
        {
            _isWalking = false;
        }
        else
        {
            _isWalking = true;
        }

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
            _animName = "Shooting";
        }
        else
        {
            Instantiate(bulletPrefab, _pos.transform.position, new Quaternion(0, -180, 0, 0));
            _animName = "Shooting e";
        }

        StartCoroutine(Anim("isShooting", _animName));

    }
    IEnumerator Anim(string boolName, string animName)
    {
        Debug.Log("entrou");
        _animator.SetBool(boolName, true);

        yield return null;

        while (_animator.GetCurrentAnimatorStateInfo(0).IsName(animName) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            Debug.Log("esperou");
            yield return null;
        }

        _animator.SetBool(boolName, false);

        Debug.Log("saiu");

        yield break;
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
