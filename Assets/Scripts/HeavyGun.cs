using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.Rendering.HDROutputUtils;

public class HeavyGun : MonoBehaviour
{
    public static HeavyGun Instance;

    [Header("Referências")]
    [SerializeField] private Transform _shootPos;
    [SerializeField] private GameObject _shootPrefab;
    [SerializeField] private LineRenderer _lR;


    [Header("Informações")]
    [SerializeField] private int _idWeapon;
    [SerializeField] private float _minF;
    [SerializeField] private float _maxF;
    [SerializeField] private float _forceAdd;
    [SerializeField] private float _shootSpeed;
    [SerializeField] private float _ammoCapacity;

    [Header("Trajetória")]
    [SerializeField] private int _trajectorySteps;
    [SerializeField] private float _timeStep;

    private GameObject _shoot;

    private string _axisKey;
    private float _actualForce, _ammo;
    private float _axisY;
    private bool _operative, _reloading;

    static public float force;

    private Rigidbody2D _rb;

    private KeyCode _shootK, _reloadK;

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

        _ammo = _ammoCapacity;
    }

    void Update()
    {
  
    }

    void IsOn()
    {
        _axisY = Input.GetAxis(_axisKey);
        _actualForce += _axisY * _forceAdd * Time.deltaTime;
        _actualForce = Mathf.Clamp(_actualForce, _minF, _maxF);
    }

    void ShowTrajectory()
    {
        Vector2 _direction = _shootPos.right.normalized;
        Vector2 _velocity = _direction * _actualForce;
        Vector2 _startPos = _shootPos.position;
        _lR.positionCount = _trajectorySteps;

        for (int i = 0; i < _trajectorySteps; i++) {
            float t = i * _timeStep; // tempo para esse ponto
            Vector2 pos = _startPos + _velocity * t + 0.5f * Physics2D.gravity * t * t;
            _lR.SetPosition(i, pos);
        }
    }

    void Shoot()
    {
        _shoot = Instantiate(_shootPrefab, _shootPos.position, Quaternion.identity);
        Rigidbody2D rb = _shoot.GetComponent<Rigidbody2D>();
        if (rb != null) {
            Vector2 forceDir = _shootPos.right.normalized * _actualForce;
            rb.AddForce(forceDir, ForceMode2D.Impulse);
        }
        //_ammo--;
    }

    IEnumerator Reload()
    {
        _reloading = true;

        yield return new WaitForSeconds(5f);
        _ammo = _ammoCapacity;

        // CallAmmoChange();
        _reloading = false;

        yield break;
    }

    private void GunSystem(int _p)
    {
        if (_p == 0) {
            _axisKey = "Vertical-P1";
        } else {
            _axisKey = "Vertical-P2";
        }

        _shootK = Player.sKey;
        _reloadK = Player.rKey;

        IsOn();
        ShowTrajectory();
        if (Input.GetKeyDown(Player.sKey) && !_reloading && (_ammo > 0)) {
            Shoot();
        } else if (Input.GetKey(Player.rKey) && !_reloading) {
            StartCoroutine(Reload());
        }
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
