using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Armor : MonoBehaviour
{
    [SerializeField] private int _ArmorPoints;

    private void OnTriggerEnter2D(Collider2D other)
    {
    PlayerMover player = other.GetComponent<PlayerMover>();
    if (player!=null)
        {
        player.AddArmor(_ArmorPoints);
        Destroy(gameObject);
        }
    }
}
