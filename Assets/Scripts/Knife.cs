using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Knife : MonoBehaviour
{

    private SpriteRenderer _sR;
    private Animation _anim;

    private bool _isFlipped;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInParent<Animation>();
        _sR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!_anim.isPlaying)
        {
            _isFlipped = Player.isFlipped2;
            _sR.flipX = _isFlipped;
        }
    }
}
