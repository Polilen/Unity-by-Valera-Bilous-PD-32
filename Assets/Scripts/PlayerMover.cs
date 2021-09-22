using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    [SerializeField] private float _speed;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private float _jumpForce;

    [SerializeField] private Transform _groundChecker;
    [SerializeField] private float _groundCheckerRadius;

    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private LayerMask _whatIsCell;
    [SerializeField] private Collider2D _headCollider;
    [SerializeField] private float _headCheckerRadius;
    [SerializeField] private Transform _headChecker;

    [Header(("Animation"))]
    [SerializeField] private Animator _animator;

    [SerializeField] private string _runAnimatorKey;
    [SerializeField] private string _jumpAnimatorKey;
    [SerializeField] private string _crouchAnimatorKey;
    [SerializeField] private string _attackAnimatorKey;
    [SerializeField] private string _castAnimatorKey;
    private float _horizontalDirection;
    private float _verticalDirection;

    private bool _jump;

    private bool _crawl;

    public bool Canclimb { private get; set; }
    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        _horizontalDirection = Input.GetAxisRaw("Horizontal");
        _verticalDirection = Input.GetAxisRaw("Vertical");
        _animator.SetFloat(_runAnimatorKey,Mathf.Abs(_horizontalDirection));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jump=true;
        }
        if (_horizontalDirection > 0 && _spriteRenderer.flipX)
        {
            _spriteRenderer.flipX = false;
        }
        else if(_horizontalDirection<0 && !_spriteRenderer.flipX)
        {
            _spriteRenderer.flipX = true;
        }

        _crawl = Input.GetKey(KeyCode.C);

    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = new Vector2(_horizontalDirection * _speed, _rigidbody.velocity.y);
        // Lab 1
        //_rigidbody.AddForce(new Vector2(50_direction,0),ForceMode2D.Impulse);
        //_rigidbody.MovePosition(_rigidbody.position+new Vector2(_direction1,0));
        //_transform.position += new Vector3(_direction1,0,0);
        //_transform.Translate(new Vector3(1_direction,0,0));
        if (Canclimb)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _verticalDirection * _speed);
            _rigidbody.gravityScale = 0;
        }
        else
        {
            _rigidbody.gravityScale = 5;
        }

            bool canJump = Physics2D.OverlapCircle(_groundChecker.position , _groundCheckerRadius,_whatIsGround);
        bool canStand = !Physics2D.OverlapCircle(_headChecker.position , _headCheckerRadius,_whatIsCell);
        _headCollider.enabled = !_crawl && canStand;
        if (_jump && canJump)
        {
            _rigidbody.AddForce(Vector2.up * _jumpForce);
            _jump = false;
        }
        _animator.SetBool(_jumpAnimatorKey,!canJump);
        _animator.SetBool(_crouchAnimatorKey,!_headCollider.enabled);
        _animator.SetBool(_attackAnimatorKey,Input.GetKey(KeyCode.Mouse0));
        _animator.SetBool(_castAnimatorKey,Input.GetKey(KeyCode.U));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundChecker.position,_groundCheckerRadius);
        Gizmos.color=Color.green;
        Gizmos.DrawWireSphere(_headChecker.position,_headCheckerRadius);
    }

    public void AddHp(int hpPoints)
    {
        Debug.Log(message:"Hp raised " + hpPoints);
    }
    public void AddMana(int ManaPoints)
    {
        Debug.Log(message:"Mana raised " + ManaPoints);
    }
    public void AddArmor(int ArmorPoints)
    {
        Debug.Log(message:"Armor raised " + ArmorPoints);
    }
}
