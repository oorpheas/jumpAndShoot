using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject _reference;
    public float bulletSpeed;

    private bool _isFlipped;
    private float Timer;

    void Start()
    {


        _isFlipped = Player.isFlipped;
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        Moviment();
        DestroyBullet();        
    }

    void Moviment()
    {
        if (_isFlipped)
        {
            transform.position += transform.right * bulletSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += transform.right * bulletSpeed * Time.deltaTime;
        }
    }

    void DestroyBullet()
    {
        if (Timer > 5)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
            Destroy(gameObject);
    }
}
