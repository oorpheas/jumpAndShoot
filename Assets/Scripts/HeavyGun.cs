using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyGun : MonoBehaviour
{
    public static HeavyGun Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void GunSystem()
    {

    }

    private void OnEnable()
    {
        Player.PlayerInteracted += GunSystem;
    }

    private void OnDisable()
    {
        Player.PlayerInteracted -= GunSystem;
    }
}
