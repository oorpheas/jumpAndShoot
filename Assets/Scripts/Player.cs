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
    // VARIAVEIS ESTÁTICAS
    public static event Action<int, string> OnAmmoChanged;
    public static event Action<Collider2D, PolygonCollider2D, bool> PlayerPassed;
    public static event Action<Collider2D, PolygonCollider2D> PlayerWantedPass;
    public static event Action<int, Transform> PlayerInteracted;
    public static bool isFlipped, playerStats;
    public static string ammo;

    // CAMPOS VÍSIVEIS NO INSPECTOR
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

    [Header("Defina as Teclas")]
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private KeyCode _downKey;
    [SerializeField] private KeyCode _shootKey;
    [SerializeField] private KeyCode _reloadKey;
    [SerializeField] private KeyCode _interactKey;
    [SerializeField] private KeyCode _attackKey;

    // CAMPOS PRIVADOS;
    private int _ammo, _id;
    private float _axisX, _timer;
    private string _animName, _input;
    private bool _isGrounded, _inInteractionArea, _isWalking, _isReloading, _isFlipped, _isRight, _playerOne, _isAiming;

    private Rigidbody2D _rb2d;
    private Transform _gunL, _gunR, _armaUsada, _heavygun;
    private Animator _animator;
    private Animation _anim;
    private GameObject _spawn;
    private Collider2D _self;
    private PolygonCollider2D _collider;

    static public KeyCode shootK, interactionK;

    void Awake()
    {
        _isAiming = false;
        _ammo = _ammoCapacity;

        _playerOne = (this.jogador == 0);
        if (_playerOne) {
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
        _self = gameObject.GetComponent<Collider2D>();

        _gunL = GetComponentInChildren<Transform>().Find("gunL");
        _gunR = GetComponentInChildren<Transform>().Find("gunR");

        _armaUsada = _gunR;

        shootK = _shootKey;
        interactionK = _interactKey;
    }

    void Update()
    {
        if (!_isAiming) {
            SetAmmo();
            Shoot();
        }

        Interacted();
    }

    void FixedUpdate()
    {
        if (!_isAiming) {
            Jump();
            SetMoviment();
        } else {
            _animator.SetBool("isWalking", false);
        }
    }

    private void Spawn(string spawn)
    {
        _spawn = GameObject.FindGameObjectWithTag(spawn);
        gameObject.transform.position = _spawn.transform.position;
    }

    // CHAMA EVENTOS
    private void CallAmmoChange()
    {
        ammo = (_ammo + "/" + _ammoCapacity);
        OnAmmoChanged?.Invoke(_id, ammo);
    }

    private void CallInteract()
    {
        PlayerInteracted?.Invoke(_id, _heavygun);
    }

    // METODOS

    void Moviment(float axis)
    {
        _rb2d.velocity = new Vector2(axis * _speed, _rb2d.velocity.y); // em Y ele está recebendo a velocidade atribuida (ou seja, gravidade);_isWalking = (_rb2d.velocity.x != 0f); 
        _isWalking = (_rb2d.velocity.x != 0);

        if (axis != 0) {
            SetFlip(axis);
        }

        _armaUsada = _isFlipped ? _gunL.transform : _gunR.transform;
    }

    void Jump()
    {
        if (Input.GetKey(_jumpKey) && _isGrounded) { // qualquer variavel bool é entendida como TRUE dentro de verificações, exceto se for !bool
            Vector2 _jump = new Vector2(0f, jumpForce);
            _rb2d.AddForce(_jump, ForceMode2D.Impulse);
        }
    }

    void Interacted()
    {
        if (_inInteractionArea && Input.GetKeyDown(_interactKey)) {
            _isAiming = !_isAiming;
            playerStats = _isAiming;
            if (_isAiming) {
                CallInteract();
            } 
        }
    }

    void Shoot()
    {
        if ((_ammo > 0) && Input.GetKeyDown(_shootKey) && !_isReloading) {
            SetShoot(_armaUsada, _isFlipped);
            --_ammo;
            CallAmmoChange();
        } else if (Input.GetKeyDown(_reloadKey)) {
            StartCoroutine(Reload());
        }
    }

    // SETs
    void SetAmmo()
    {
        ammo = (_ammo + "/" + _ammoCapacity);
    }

    void SetFlip(float a)
    {
        _isFlipped = (a < 0);
        _animator.SetBool("isFlipped", _isFlipped);
    }

    void SetMoviment()
    {
        _axisX = Input.GetAxis(_input);
        Moviment(_axisX);

        _animator.SetBool("isWalking", _isWalking);
        _animator.SetBool("isIdle", !_isWalking);
        //Debug.LogWarning("player está: \n " +
        //    "idle? = " + _animator.GetBool("isIdle") +
        //    " || andando? = " + _animator.GetBool("isWalking") +
        //    " || virado? " + _animator.GetBool("isFlipped"));
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

    void Knife()
    {
        if (!_anim.isPlaying && Input.GetKeyUp(_attackKey)) {
            _anim.Play("attack");
        }
    }

    // RECARGA
    IEnumerator Reload()
    {
        _isReloading = true;

        //for (int i = _ammo; i <= _ammoCapacity; i++) {
        //    _ammo = i;
        //    int lastNum = _ammo;
        //    yield return new WaitForSeconds(0.5f);
        //    CallAmmoChange();
        //    Debug.Log(_ammo);
        //}

        yield return new WaitForSeconds(2f);
        _ammo = _ammoCapacity;

        CallAmmoChange();
        _isReloading = false;

        yield break;
    }

    // ANIMAÇÃO
    IEnumerator Anim(string boolName, string animName)
    {
        _animator.SetBool(boolName, true);

        yield return null;

        while (_animator.GetCurrentAnimatorStateInfo(0).IsName(animName) &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        _animator.SetBool(boolName, false);

        yield break;
    }

    // COLISSÕES
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ground")) {
            _isGrounded = true;
        }

        if (other.gameObject.CompareTag("sensor")){
            if (Input.GetKey(_downKey)) {
                _collider = other.gameObject.GetComponent<PolygonCollider2D>();
                PlayerWantedPass?.Invoke(_self, _collider);

            }
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("sensor")) {
            _collider = other.gameObject.GetComponent<PolygonCollider2D>();
            _isRight = (_axisX > 0);
            PlayerPassed?.Invoke(_self, _collider, !_isRight);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("arma") || other.gameObject.CompareTag("arma2")) {
            _inInteractionArea = true;
            _heavygun = other.GetComponentInChildren<Transform>().Find("pos");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("arma") || other.gameObject.CompareTag("arma2")) {
            _inInteractionArea = false;
        }
    }

}



