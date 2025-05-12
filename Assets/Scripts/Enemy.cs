using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float enemySpeedMin, enemySpeedMax, jumpForceMin, jumpForceMax, attackRangeMin, attackRangeMax, cooldownMin, cooldownMax;

    static public bool isAttacking;

    private GameObject[] _player;
    private float _enemySpeed, _jumpForce, _distance, _attackRange, _cooldown, _timer;

    private Rigidbody2D _rb2d;
    private SpriteRenderer _self;

    private bool _isGrounded;
    private int _rS, _choosePlayer;

    [SerializeField]
    private Color[] shirts;

    private Color _shirt;

    private Animator _animator;


    // Start is called before the first frame update
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _self = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        FindPlayer();

        _enemySpeed = Random.Range(enemySpeedMin, enemySpeedMax);
        _jumpForce = Random.Range(jumpForceMin, jumpForceMax);
        _attackRange = Random.Range(attackRangeMin, attackRangeMax);
        _cooldown = Random.Range(cooldownMin, cooldownMax);

        _rS = Random.Range(0, shirts.Length - 1);

        _shirt = new Color(shirts[_rS].r, shirts[_rS].g, shirts[_rS].b);
        Debug.Log(_rS);
        _self.material.SetColor("_color_A", _shirt);

        Debug.Log("o range é" + _attackRange);
        Debug.Log("o cooldown é" + _cooldown);
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        Debug.Log("está atacando?" + isAttacking);

        if (_player[_choosePlayer] != null) {
            MoveToPlayer();
            CorrectAxis();

        } else {
            FindPlayer();
        }

        if (_cooldown < _timer) {
            AttackPlayer();
        } else {
            Debug.Log("está em cooldown");
        }

    }

    void FindPlayer()
    {
        _player = GameObject.FindGameObjectsWithTag("Player");
        if (_player != null) {
            _choosePlayer = Random.Range(0, _player.Length);
        } else {
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isDancing", true);
        }
    }

    void MoveToPlayer()
    {
        transform.position = Vector2.MoveTowards(gameObject.transform.position, _player[_choosePlayer].transform.position, _enemySpeed * Time.deltaTime);
        _distance = Vector2.Distance(transform.position, _player[_choosePlayer].transform.position);
        _animator.SetBool("isWalking", true);
    }

    void CorrectAxis()
    {
        if ((_player[_choosePlayer].transform.position.x - _self.transform.position.x < _self.transform.position.x)) {
            _self.flipX = true;
        } else
            _self.flipX = false;
    }

    void AttackPlayer()
    {
        if (_distance >= _attackRange) {
            isAttacking = true;
            _timer = 0;
            Debug.LogWarning("esta atacando");
        } else
            Debug.Log("atacando, porém fora do range");
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
        if (other.gameObject.CompareTag("bullet") || other.gameObject.CompareTag("knife")) {
            GameManager.score++;
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("window_area")) {
            if (_isGrounded) {
                Vector2 _jump = new Vector2(0f, _jumpForce);
                _rb2d.AddForce(_jump, ForceMode2D.Impulse);

            }
        }
    }
}
