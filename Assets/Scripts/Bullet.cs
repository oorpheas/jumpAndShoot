using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    private float _timer;

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        Moviment();
        DestroyBullet();
    }

    void Moviment()
    {
        transform.position += transform.right * bulletSpeed * Time.deltaTime;
    }

    void DestroyBullet()
    {
        if (_timer > 3f) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
            Destroy(gameObject);
    }
}
