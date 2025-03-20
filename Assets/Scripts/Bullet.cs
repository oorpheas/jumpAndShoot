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
        _scriptP = _player.GetComponent<Player>();

        if (_player.playerAxis < 0)
        {
            _sR.flipX = true;
        }
        else if (axisX > 0)
        {
            _sR.flipX = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(10f, 0, 0);
        //Moviment();
        //if (_isFlipped)
        //{
        //    if (transform.position.x < leftLimit)
        //        Destroy(gameObject);
        //}
        //else
        //{
        //    if (transform.position.x > rightLimit)
        //        Destroy(gameObject);
        //}
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
            Destroy(gameObject);
    }
}
