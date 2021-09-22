using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Movetolvltwo : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player!=null)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Destroy(gameObject);   
            }
        }
    }
}
