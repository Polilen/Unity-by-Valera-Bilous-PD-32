using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FlyEnemy : MonoBehaviour
{
    [SerializeField] private float _walkRange;
    [SerializeField] private bool _face_Right;
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private int _damage;
    [SerializeField] private int _coinsAmount;
    [SerializeField] private float _pushPower;
    [SerializeField] private int _maxHp;
    [SerializeField] private Slider _slider;
    [SerializeField] private Transform _player;
    private int _currentHp;
    private Vector2 _startPosition;
    private int _direction=1;
    private float _lastAttackTime;
    private Vector2 _drawPosition
    {
        get
        {
            if (_startPosition == Vector2.zero)
            {
                return transform.position;
            }
            else
            {
                return _startPosition;
            }
        }
    }

    private void Start()
    {
        _slider.maxValue = _maxHp;
        ChangeHp(_maxHp);
        _startPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_drawPosition, new Vector3(_walkRange*2,1,0));
    }
    private void ChangeHp(int hp)
    {
        _currentHp = hp;
        _slider.value = hp;
    }
    private void Update()
    {
        if (_face_Right && transform.position.x > _startPosition.x + _walkRange)
        {
            Flip();
        }
        else if (!_face_Right && transform.position.x < _startPosition.x - _walkRange)
        {
            Flip();
        }
    }

    private void Flip()
    {
        _face_Right = !_face_Right;
        transform.Rotate(0,180,0);
        _direction *= -1;
    }

    private void FixedUpdate()
    {
        _rigidbody2D.velocity=Vector2.right * _direction * _speed;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerMover player = other.collider.GetComponent<PlayerMover>();
        if (player != null && Time.time - _lastAttackTime > 0.2f)
        {
            _lastAttackTime = Time.time;
            player.TakeDamage(_damage,_pushPower,transform.position.x);
        }
    }
    public void TakeDamage(int damage)
    {
        PlayerMover player = _player.GetComponent<PlayerMover>();
        ChangeHp(_currentHp-damage);
        if (_currentHp <= 0)
        {
            player.CoinsAmount += _coinsAmount;
            Destroy(gameObject);
        }
    }
}