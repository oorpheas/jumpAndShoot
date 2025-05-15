using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.Rendering.HDROutputUtils;

public class HeavyGun : MonoBehaviour
{
    public static HeavyGun Instance;

    [Header("Referências")]
    [SerializeField] private GameObject _shootPrefab;
    [SerializeField] private LineRenderer _lR;

    [Header("Informações")]
    [SerializeField] private float _minF;
    [SerializeField] private float _maxF;
    [SerializeField] private float _forceAdd;
    [SerializeField] private float _shootSpeed;

    [Header("Trajetória")]
    [SerializeField] private int _trajectorySteps;
    [SerializeField] private float _timeStep;
    [SerializeField] private Transform _shootPos;

    private KeyCode _shootKey, _interactKey;
    private GameObject _shoot;

    private string _axisKey;
    private float _actualForce;
    private float _axisY;
    private int _whichGun;
    private bool _operative;

    private Vector2 _direction;
    private Vector2 _velocity;
    private Vector2 _startPos;
    private Vector2 _forceDir;
    private Vector2 _pos;

    private Rigidbody2D _rb;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

    }

    void Start()
    {
        _lR = GetComponent<LineRenderer>();
        _actualForce = _minF;
    }

    void Update()
    {
        if (Player.playerStats) {
            _lR.enabled = true;
            IsOn();
            ShowTrajectory();
            if (Input.GetKeyDown(_shootKey)) {
                Shoot();
            }
        } else {
            _lR.enabled = false;
        }
    }

    void IsOn()
    {
        _axisY = Input.GetAxis(_axisKey);
        _actualForce += _axisY * _forceAdd * Time.deltaTime;
        _actualForce = Mathf.Clamp(_actualForce, _minF, _maxF);

        ShowTrajectory();
        if (Input.GetKeyDown(_shootKey)) {
            Shoot();
        }
    }

    void ShowTrajectory()
    {
        if (_shootPos?.position.x > 0) {
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

    private void GunSystem(int _p, Transform _t)
    {
        if (_p == 0) {
            _axisKey = "Vertical-P1";
        } else {
            _axisKey = "Vertical-P2";
        }

        _shootPos = _t;
        _shootKey = Player.shootK;
        _interactKey = Player.interactionK;
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
