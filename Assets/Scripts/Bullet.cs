using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    public float bulletSpeed, rightLimit, leftLimit;

    private bool _isFlipped;

    private GameObject _player;
    private SpriteRenderer _sRplayer;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _sRplayer = _player.GetComponent<SpriteRenderer>();
        if (_sRplayer.flipX)
        {
            _isFlipped = true;
        }
        else
        {
            _isFlipped = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Moviment();
        if (_isFlipped )
        {
            if (transform.position.x < leftLimit)
                Destroy(gameObject);
        }
        else 
        {
            if (transform.position.x > rightLimit)
                Destroy(gameObject);
        }
    }

    void Moviment()
    {
        if (_isFlipped)
        {
            transform.position += -transform.right * bulletSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += transform.right * bulletSpeed * Time.deltaTime;
        }
    }
}
