using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RangedEnemy : MonoBehaviour
{
    [SerializeField] private float _attackRange;
    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Rigidbody2D _bullet;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private bool _faceRight;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _coinsAmount;
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _enemySystem;
    [SerializeField] private Transform _player;
    private int _currentHp;
    private bool _canShoot;
    
    
    [Header("Animation")] [SerializeField] private Animator _animator;
    [SerializeField] private string _shootAnimationKey;

    private void Start()
    {
        _slider.maxValue = _maxHp;
        ChangeHp(_maxHp);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color=Color.green;       
        Gizmos.DrawWireCube(transform.position,new Vector3(_attackRange,1,0));
    }

    private void ChangeHp(int hp)
    {
        _currentHp = hp;
        _slider.value = hp;
    }
    private void FixedUpdate()
    {
        
        if (_canShoot)
        {
            return;
        }
        CheckIfCanShoot();
    }

    private void CheckIfCanShoot()
    {
        Collider2D player = Physics2D.OverlapBox(transform.position, new Vector2(_attackRange, 1), 0,_whatIsPlayer);
        if (player != null)
        {
            _canShoot = true;
            StartShoot(player.transform.position);
        }
        else
        {
            _canShoot = false;
        }
    }
    private void StartShoot(Vector2 playerPosition)
    {
        if (transform.position.x > playerPosition.x && _faceRight || transform.position.x < playerPosition.x && !_faceRight)
        {
            _faceRight = !_faceRight;
            transform.Rotate(0,180,0);
        }
        _animator.SetBool(_shootAnimationKey,true);
    }
    public void Shoot()
    {
        Rigidbody2D bullet = Instantiate(_bullet);
        bullet.position = _muzzle.position;
        bullet.velocity = _projectileSpeed * transform.right;
        _animator.SetBool(_shootAnimationKey,false);
        Invoke(nameof(CheckIfCanShoot),1f);
    }

    public void TakeDamage(int damage)
    {
        PlayerMover player = _player.GetComponent<PlayerMover>();
        ChangeHp(_currentHp-damage);
        if (_currentHp <= 0)
        {
            player.CoinsAmount += _coinsAmount;
            Destroy(_enemySystem);
        }
    }
}
