using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.Rendering.HDROutputUtils;

public class HeavyGun : MonoBehaviour
{
    public static HeavyGun Instance;
    public static event Action<int, string> OnAmmoChanged2;

    [Header("Referências")]
    [SerializeField] private GameObject _shootPrefab;
    [SerializeField] private LineRenderer _lR;
    [SerializeField] private Text[] _info;

    [Header("Informações")]
    [SerializeField] private float _minF;
    [SerializeField] private float _maxF;
    [SerializeField] private float _forceAdd;
    [SerializeField] private float _shootSpeed;
    [SerializeField] private int _ammoCapacity;
    private int _ammo;
    private int _i;

    [Header("Trajetória")]
    [SerializeField] private int _trajectorySteps;
    [SerializeField] private float _timeStep;
    [SerializeField] private Transform _shootPos;

    private KeyCode _shootKey, _reloadKey;
    private GameObject _shoot;

    private string _axisKey;
    private float _actualForce;
    private float _axisY;
    private int _whichGun;
    private bool _operative, _isReloading;

    private Vector2 _direction;
    private Vector2 _velocity;
    private Vector2 _startPos;
    private Vector2 _forceDir;
    private Vector2 _pos;

    private Rigidbody2D _rb;

    public static string ammo;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _lR = GetComponent<LineRenderer>();
        _actualForce = _minF;
        _ammo = _ammoCapacity;
    }

    void Update()
    {
        if (Player.playerStats) {
            _lR.enabled = true;
            IsOn();
            ShowTrajectory();
            if ((_ammo > 0) && Input.GetKeyDown(_shootKey) && !_isReloading) {
                Shoot();
                --_ammo;
                CallAmmoChange();
            } else if (Input.GetKeyDown(_reloadKey)) {
                StartCoroutine(Reload());
            }
        } else {
            _lR.enabled = false;
            _info[_i].enabled = false;
        }
    }

    void IsOn()
    {
        _axisY = Input.GetAxis(_axisKey);
        _actualForce += _axisY * _forceAdd * Time.deltaTime;
        _actualForce = Mathf.Clamp(_actualForce, _minF, _maxF);

    }

    void ShowTrajectory()
    {
        _info[_i].enabled = true;
        if (_shootPos.position.x > 0) {
            _direction =_shootPos.right.normalized;
        } else {
            _direction = -_shootPos.right.normalized;
        }

        _velocity = _direction * _actualForce;
        _startPos = _shootPos.position;
        _lR.positionCount = _trajectorySteps;

        for (int i = 0; i < _trajectorySteps; i++) {
            float t = i * _timeStep; // tempo para esse ponto
            _pos = _startPos + _velocity * t + 0.5f * Physics2D.gravity * t * t;
            _lR.SetPosition(i, _pos);
        }
    }

    void AmmoOn()
    {
        if (_shootPos.position.x < 0) {
            
            _i = 0;
        } else {
            _i = 1;
        }

        Debug.Log(_i);
    }

    void Shoot()
    {
        _shoot = Instantiate(_shootPrefab, _shootPos.position, Quaternion.identity);
        Rigidbody2D rb = _shoot.GetComponent<Rigidbody2D>();
        if (rb != null) {
            if (_shootPos.position.x > 0) {
                _forceDir = _shootPos.right.normalized * _actualForce;
            } else {
                _forceDir = -_shootPos.right.normalized * _actualForce;
            }
            rb.AddForce(_forceDir * _shootSpeed, ForceMode2D.Impulse);
        }
    }

    private void CallAmmoChange()
    {
        ammo = (_ammo + "/" + _ammoCapacity);
        OnAmmoChanged2?.Invoke(_i, ammo);
    }

    IEnumerator Reload()
    {
        _isReloading = true;

        yield return new WaitForSeconds(5f);
        _ammo = _ammoCapacity;

        CallAmmoChange();
        _isReloading = false;

        yield break;
    }

    private void GunSystem(int _p, Transform _t, KeyCode _s, KeyCode _r)
    {
        if (_p == 0) {
            _axisKey = "Vertical-P1";
        } else {
            _axisKey = "Vertical-P2";
        }

        _shootPos = _t;
        _shootKey = _s;
        _reloadKey = _r;

        AmmoOn();
    }

    private void OnEnable()
    {
        Player.PlayerInteracted += GunSystem;
    }

    private void OnDisable()
    {
        Player.PlayerInteracted -= GunSystem;
    }
}
