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
    void FixedUpdate()
    {
        SetFlip();
    }

    void SetFlip()
    {
        if (Player.isFlipped2)
        {
            Quaternion _flipAnim = transform.localRotation;
            _flipAnim.x *= -1;
            Vector2 _flipAxis = transform.localPosition;
            _flipAxis.x *= -1;
            transform.localRotation = _flipAnim;
            transform.localPosition = _flipAxis;

            Debug.Log(Player.isFlipped2);
        }
    }
}
