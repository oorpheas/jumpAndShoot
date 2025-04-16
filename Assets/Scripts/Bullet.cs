using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    private float Timer;

    void Start()
    {
        
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
        transform.position += transform.right * bulletSpeed * Time.deltaTime;
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
