using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    static public int pID;
    static public bool isFlipped;

    public int playerID;

    public float jumpForce;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private int _speed, _ammoCapacity;
    private int[] _ammo = new int[1];

    private float _axisX, _timer;

    private bool _isGrounded, _isWalking;

    private Rigidbody2D _rb2d;

    private SpriteRenderer _sR;

    private Transform _gunL, _gunR, _armaUsada;

    private Animator _animator;

    private GameObject _spawn;

    private string _animName;

    // Start is called before the first frame update
    void Start()
    {
        // define ID publico
        pID = playerID;
        _ammo[0] = _ammoCapacity;
        //_ammo[1] = _ammoCapacity;

        _animator = GetComponent<Animator>();
        _rb2d = GetComponent<Rigidbody2D>();
        _sR = GetComponent<SpriteRenderer>();

        _gunL = GetComponentInChildren<Transform>().Find("gunL");
        _gunR = GetComponentInChildren<Transform>().Find("gunR");
        _armaUsada = _gunR.transform;

        if (playerID == 1) {
            _spawn = GameObject.FindGameObjectWithTag("playerSpawn");
            gameObject.transform.position = _spawn.transform.position;
        } else if (playerID == 2) {
            _spawn = GameObject.FindGameObjectWithTag("playerSpawn2");
            gameObject.transform.position = _spawn.transform.position;

            //_anim = gameObject.GetComponent<Animation>();
        } else {
            Debug.LogWarning("não tem controles associados a esse ID");
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Jump();
        SetMoviment();
    }

    void Update()
    {
        Shoot();
    }

    void SetFlip()
    {
        _animator.SetBool("isFlipped", isFlipped);
    }

    void SetMoviment()
    {
        if (playerID == 1) {
            _axisX = Input.GetAxis("Horizontal-P1");
            Moviment(_axisX);
        } else if (playerID == 2) {
            _axisX = Input.GetAxis("Horizontal-P2");
            Moviment(_axisX);
        } else {
            Debug.LogWarning("não tem controles associados a esse ID");
        }

        _animator.SetBool("isWalking", _isWalking);
        _animator.SetBool("isIdle", !_isWalking);
        Debug.LogWarning("player está: \n " +
            "idle = " + _animator.GetBool("isIdle") +
            " || andando = " + _animator.GetBool("isWalking") +
            " || tá virado? " + _animator.GetBool("isFlipped"));
    }

    void Moviment(float axis)
    {
        _rb2d.velocity = new Vector2(axis * _speed, _rb2d.velocity.y); // em Y ele está recebendo a velocidade atribuida (ou seja, gravidade);_isWalking = (_rb2d.velocity.x != 0f); 
        _isWalking = (_rb2d.velocity.x != 0);

        if (axis < 0) {
            _armaUsada = _gunL.transform;
            isFlipped = true;
        } else if (axis > 0) {
            _armaUsada = _gunR.transform;
            isFlipped = false;
        }

        SetFlip();
    }

    void Jump()
    {
        if (playerID == 1) {
            if (Input.GetKey(KeyCode.W) && _isGrounded) // qualquer variavel bool é entendida como TRUE dentro de verificações, exceto se for !bool
            {
                Vector2 _jump = new Vector2(0f, jumpForce);
                _rb2d.AddForce(_jump, ForceMode2D.Impulse);
            }
        } else if (playerID == 2) {
            if (Input.GetKey(KeyCode.UpArrow) && _isGrounded) // qualquer variavel bool é entendida como TRUE dentro de verificações, exceto se for !bool
            {
                Vector2 _jump = new Vector2(0f, jumpForce);
                _rb2d.AddForce(_jump, ForceMode2D.Impulse);
            }
        } else {
            Debug.LogWarning("não tem controles associados a esse ID");
        }
    }

    void Shoot()
    {
        if (playerID == 1) {
            if ((_ammo[0] > 0) && Input.GetKeyDown(KeyCode.LeftControl)) {
                SetShoot(_armaUsada, isFlipped);
                --_ammo[0];
                Debug.Log(_ammo[0]);
            } else if (Input.GetKey(KeyCode.R)) {
                StartCoroutine(Reload());
            }
        } else if (playerID == 2) {
            if (Input.GetKeyUp(KeyCode.RightControl)) {
                SetShoot(_armaUsada, isFlipped);
            }
        } else {
            Debug.LogWarning("não tem controles associados a esse ID");
        }
    }
    IEnumerator Reload()
    {
        for (int i = 0; i <= _ammoCapacity; i++) {
            _ammo[0] = i;
            yield return new WaitForSeconds(0.5f);
            Debug.Log(_ammo[0]);
        }

        yield break;
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
        if (!_flip) {
            _animName = "Shooting";
            StartCoroutine(Anim("isShooting", _animName));
            Instantiate(bulletPrefab, _pos.transform.position, transform.rotation);
        } else {
            _animName = "Shooting e";
            StartCoroutine(Anim("isShooting", _animName));
            Instantiate(bulletPrefab, _pos.transform.position, new Quaternion(0, -180, 0, 0));
        }
    }

    IEnumerator Anim(string boolName, string animName)
    {
        Debug.Log("entrou");
        _animator.SetBool(boolName, true);

        yield return null;

        while (_animator.GetCurrentAnimatorStateInfo(0).IsName(animName) &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            Debug.Log("esperou");
            yield return null;
        }

        _animator.SetBool(boolName, false);

        Debug.Log("saiu");

        yield break;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ground")) {
            _isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ground")) {
            _isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("enemy")) {
            Destroy(gameObject);
        }
    }
}
