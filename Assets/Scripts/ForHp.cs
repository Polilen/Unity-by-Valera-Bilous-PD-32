using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForHp : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private Transform _followto;
    private Quaternion rotation;

    void Awake()
    {
        rotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.rotation = rotation;
    }

    private void Update()
    {
        _canvas.position = new Vector2(_canvas.position.x, _followto.transform.position.y*1.005f);
    }
}