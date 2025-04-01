using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float enemySpeedMin, enemySpeedMax, jumpForceMin, jumpForceMax, attackRangeMin, attackRangeMax, cooldownMin, cooldownMax;
    
    public SpriteRenderer _renderer;
    
    static public bool isAttacking;
    
    private GameObject _player;
    private float _enemySpeed, _jumpForce, _distance, _attackRange, _cooldown, _timer;

    private Rigidbody2D _rb2d;
    private bool _isGrounded;

    private int _rS;

    [SerializeField]
    private Color[] shirts;

    private Animator _isWalking, _isAttacking, _isDancing;

    // Start is called before the first frame update
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();

        _player = GameObject.FindGameObjectWithTag("Player");

        _enemySpeed = Random.Range(enemySpeedMin, enemySpeedMax);
        _jumpForce = Random.Range(jumpForceMin, jumpForceMax);
        _attackRange = Random.Range(attackRangeMin, attackRangeMax);
        _cooldown = Random.Range(cooldownMin, cooldownMax);

        _rS = Random.Range(0, shirts.Length-1);

        _renderer.color = new Color(shirts[_rS].r, shirts[_rS].g, shirts[_rS].b);

        Debug.Log("o range é" + _attackRange);
        Debug.Log("o cooldown é" + _cooldown);
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        Debug.Log("está atacando?" + isAttacking);

        MoveToPlayer();

        if (_cooldown < _timer)
        {
            Debug.Log("pode atacar!");
            AttackPlayer();
            Debug.Log("atacou!");
        }
        else
            Debug.Log("cooldown!");

    }

    void MoveToPlayer()
    {
        if (_player != null)
        {
            transform.position = Vector3.MoveTowards(gameObject.transform.position, _player.transform.position, _enemySpeed * Time.deltaTime);
            _distance = Vector2.Distance(transform.position, _player.transform.position);
        }
    }

    void AttackPlayer()
    {      
        if (_distance >= _attackRange)
        {
            isAttacking = true;
            _timer = 0;
            Debug.LogWarning ("esta atacando");
        }
        else
            Debug.Log("atacando, porém fora do range");
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
        if (other.gameObject.CompareTag("bullet"))
        {
            GameManager.score++;
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("window_area"))
        {
            if (_isGrounded)
            {
                Vector2 _jump = new Vector2(0f, _jumpForce);
                _rb2d.AddForce(_jump, ForceMode2D.Impulse);

            }
        }
    }
}
