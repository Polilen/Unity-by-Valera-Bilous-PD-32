using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : MonoBehaviour
{
    [SerializeField] private int _ManaPoints;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player!=null && player.CurrentMana<100)
        {
            player.AddMana(_ManaPoints);
            Destroy(gameObject);
        }
    }
}