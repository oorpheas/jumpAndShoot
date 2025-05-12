using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class StairsSystem : MonoBehaviour
{
    public static StairsSystem Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

    }
    IEnumerator ChangeCollider(Collider2D player, PolygonCollider2D collider)
    {
        Physics2D.IgnoreCollision(player, collider, true);
        yield return new WaitForSeconds(2f);
        Physics2D.IgnoreCollision(player, collider, false);
    }

    public void UpdateCollision(Collider2D player, PolygonCollider2D collider, bool set)
    {
        Physics2D.IgnoreCollision(player, collider, set);
    }

    public void ChangeCollision(Collider2D player, PolygonCollider2D collider)
    {
        Physics2D.IgnoreCollision(player, collider, false);
        StartCoroutine(ChangeCollider(player, collider));
    }

    private void OnEnable()
    {
        Player.PlayerPassed += UpdateCollision;
        Player.PlayerWantedPass += ChangeCollision;
    }

    private void OnDisable()
    {
        Player.PlayerPassed -= UpdateCollision;
        Player.PlayerWantedPass -= ChangeCollision;
    }
}
