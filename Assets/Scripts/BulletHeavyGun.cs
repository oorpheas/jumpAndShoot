using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHeavyGun : MonoBehaviour
{
    private Animator _anim;
    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        DestroyBullet();

    }

    void DestroyBullet()
    {
        if (_timer > 3) {
            Destroy(gameObject);
        }
    }

    IEnumerator Anim()
    {
        _anim.SetBool("explosion", true);

        yield return null;

        while (_anim.GetCurrentAnimatorStateInfo(0).IsName("explosion") &&
            _anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        _anim.SetBool("explosion", false);

        yield break;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("enemy") || other.gameObject.CompareTag("ground")) { 
            //StartCoroutine(Anim());
        }
    }
}
