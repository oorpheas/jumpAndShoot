using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject _player;
    public float enemySpeedMin, enemySpeedMax, jumpForceMin, jumpForceMax;
    private float _enemySpeed, _jumpForce;

    private Rigidbody2D _rb2d;
    private bool _isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _enemySpeed = Random.Range(enemySpeedMin, enemySpeedMax);
        _jumpForce = Random.Range(jumpForceMin, jumpForceMax);
    }

    // Update is called once per frame
    void Update()
    {
        MoveToPlayer();
    }

    void MoveToPlayer()
    {
        transform.position = Vector3.MoveTowards(gameObject.transform.position, _player.transform.position, _enemySpeed * Time.deltaTime);
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
