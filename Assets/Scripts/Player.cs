using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public static event Action<int, string> OnAmmoChanged;

    [Header("Defina o Player")]
    public Jogador jogador; // cria o seletor
    public enum Jogador // cria o dropdown
    {
        PlayerOne,
        PlayerTwo
    }

    [Header("Especificações")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int _speed, _ammoCapacity;
    [SerializeField] private float jumpForce;

    private int _ammo, _id;
    private float _axisX, _timer;
    private string _animName;
    private bool _isGrounded, _isWalking, _isReloading;

    public static bool isFlipped;
    public static string ammo;

    private Rigidbody2D _rb2d;
    private Transform _gunL, _gunR, _armaUsada;
    private Animator _animator;
    private Animation _anim;
    private GameObject _spawn;

    [Header("Defina as Teclas")]
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private KeyCode _shootKey;
    [SerializeField] private KeyCode _reloadKey;
    [SerializeField] private KeyCode _attackKey;

    private string _input;

    void Awake()
    {
        _ammo = _ammoCapacity;

        bool playerOne = (this.jogador == 0) ? true : false;
        if (playerOne) {
            Spawn("playerSpawn");
            _input = "Horizontal-P1";
            _id = 0;
        } else {
            Spawn("playerSpawn2");
            _input = "Horizontal-P2";
            _id = 1;
        }

        _animator = GetComponent<Animator>();
        _rb2d = GetComponent<Rigidbody2D>();

        _gunL = GetComponentInChildren<Transform>().Find("gunL");
        _gunR = GetComponentInChildren<Transform>().Find("gunR");
        _armaUsada = _gunR.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Jump();
        SetMoviment();
    }

    void Update()
    {
        SetAmmo();
        Shoot();
    }
    private void Spawn(string spawn)
    {
        _spawn = GameObject.FindGameObjectWithTag(spawn);
        gameObject.transform.position = _spawn.transform.position;
    }

    void SetAmmo()
    {
        ammo = (_ammo + "/" + _ammoCapacity);
    }
    private void CallAmmoChange()
    {
        ammo = (_ammo + "/" + _ammoCapacity);
        OnAmmoChanged?.Invoke(_id, ammo);
    }

    void SetFlip()
    {
        _animator.SetBool("isFlipped", isFlipped);
    }

    void SetMoviment()
    {
        _axisX = Input.GetAxis(_input);
        Moviment(_axisX);

        _animator.SetBool("isWalking", _isWalking);
        _animator.SetBool("isIdle", !_isWalking);
        Debug.LogWarning("player está: \n " +
            "idle? = " + _animator.GetBool("isIdle") +
            " || andando? = " + _animator.GetBool("isWalking") +
            " || virado? " + _animator.GetBool("isFlipped"));
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
        if (Input.GetKey(_jumpKey) && _isGrounded) { // qualquer variavel bool é entendida como TRUE dentro de verificações, exceto se for !bool
            Vector2 _jump = new Vector2(0f, jumpForce);
            _rb2d.AddForce(_jump, ForceMode2D.Impulse);
        }
        
    }

    void Shoot()
    {
        if ((_ammo > 0) && Input.GetKeyDown(_shootKey) && !_isReloading) {
            SetShoot(_armaUsada, isFlipped);
            --_ammo;
            CallAmmoChange();
            Debug.Log(_ammo);
        } else if (Input.GetKeyDown(_reloadKey)) {
            StartCoroutine(Reload());
        }
    }
    IEnumerator Reload()
    {
        _isReloading = true;
        for (int i = _ammo; i <= _ammoCapacity; i++) {
            _ammo = i;
            int lastNum = _ammo;
            yield return new WaitForSeconds(0.5f);
            CallAmmoChange();
            Debug.Log(_ammo);
        }

        _isReloading = false;

        yield break;
    }

    void Knife()
    {
        if (!_anim.isPlaying && Input.GetKeyUp(_attackKey)) {
            _anim.Play("attack");
        }
    }

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
