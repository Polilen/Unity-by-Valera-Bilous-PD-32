using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxMana;
    [SerializeField] private int _maxArmor;

    [Header(("Animation"))]
    [SerializeField] private Animator _animator;

    [SerializeField] private string _runAnimatorKey;
    [SerializeField] private string _jumpAnimatorKey;
    [SerializeField] private string _crouchAnimatorKey;
    [SerializeField] private string _attackAnimatorKey;
    [SerializeField] private string _castAnimatorKey;
    [SerializeField] private string _hurtAnimationKey;
    [SerializeField] private string _JumpattackAnimatorKey;
    [SerializeField] private string _JumpcastAnimatorKey;
    [FormerlySerializedAs("_coinAmount")]
    [Header(("UI"))] 
    [SerializeField] private TMP_Text _coinAmountText; 
    [SerializeField] private Slider _hpBar;
    [SerializeField] private Slider _manaBar;
    [SerializeField] private Slider _armorBar;

    [Header("Attack")] [SerializeField] private int _swordDamage;
    [SerializeField] private Transform _swordAttackPoint;
    [SerializeField] private float _swordAttackRadius;
    [SerializeField] private LayerMask _whatIsEnemy;
    [SerializeField] private int _skillDamage;
    [SerializeField] private Transform _skillCastPoint;
    [SerializeField] private float _skillLength;
    [SerializeField] private LineRenderer _castLine;
    
    [SerializeField]private bool _faceRight;
    private float _horizontalDirection;
    private float _verticalDirection;
    private float _lastPushTime;
    private bool _jump;
    private bool _crawl;
    private bool _needToAttack;
    private bool _needToCast;
    private int _coinsAmount;

    public int CoinsAmount
    {
        get => _coinsAmount;
        set
        {
            _coinsAmount = value;
            _coinAmountText.text = value.ToString();
        }
    }
    private int _currentHp;
    private int CurrentHp
    {
        get => _currentHp;
        set
        {
            _currentHp = value;
            _hpBar.value = _currentHp;
        }
    }
    private int _currentMana;
    public int CurrentMana
    {
        get => _currentMana;
        set
        {
            _currentMana = value;
            _manaBar.value = _currentMana;
        }
    }
    private int _currentArmor;
    public int CurrentArmor
    {
        get => _currentArmor;
        set
        {
            _currentArmor = value;
            _armorBar.value = _currentArmor;
        }
    }
    public bool Canclimb { private get; set; }
    // Start is called before the first frame update
    private void Start()
    {
        CoinsAmount = 0;
        _armorBar.maxValue = _maxArmor;
        CurrentArmor = _maxArmor;
        _manaBar.maxValue = _maxMana;
        CurrentMana = _maxMana;
        _hpBar.maxValue = _maxHp;
        CurrentHp = _maxHp;
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

        if (Input.GetKey(KeyCode.Mouse0))
        {
            _needToAttack = true;
        }
        if (Input.GetKey(KeyCode.U))
        {
            _needToCast = true;
        }
        if (_horizontalDirection > 0 && !_faceRight) 
        {
            Flip();
        }
        else if(_horizontalDirection < 0 && _faceRight) 
        {
            Flip();
        }
        

        _crawl = Input.GetKey(KeyCode.C);
    }
    private void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
    }

    private void FixedUpdate()
    {
        bool canJump = Physics2D.OverlapCircle(_groundChecker.position , _groundCheckerRadius,_whatIsGround);
        if (_animator.GetBool(_hurtAnimationKey))
        {
            if (Time.time - _lastPushTime > 0.2f && canJump)
            {
                _animator.SetBool(_hurtAnimationKey, false);
            }

            _needToAttack = false;
            _needToCast = false;
            return;
        }
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
        
        bool canStand = !Physics2D.OverlapCircle(_headChecker.position , _headCheckerRadius,_whatIsCell);
        _headCollider.enabled = !_crawl && canStand;
        if (_jump && canJump)
        {
            _rigidbody.AddForce(Vector2.up * _jumpForce);
            _jump = false;
        }
        if (CurrentMana > 0)
        {
            _animator.SetBool(_castAnimatorKey,Input.GetKey(KeyCode.U));
            _animator.SetBool(_JumpcastAnimatorKey,Input.GetKey(KeyCode.U));
        }

        if (CurrentMana <= 0)
        {
            _animator.SetBool(_castAnimatorKey,false);
            _animator.SetBool(_JumpcastAnimatorKey,false);
        }
        _animator.SetBool(_jumpAnimatorKey,!canJump);
        _animator.SetBool(_crouchAnimatorKey,!_headCollider.enabled);
        _animator.SetBool(_JumpattackAnimatorKey,Input.GetKey(KeyCode.Mouse0));
        _animator.SetBool(_attackAnimatorKey,Input.GetKey(KeyCode.Mouse0));
        if (Input.GetKey(KeyCode.U))
        {
            CurrentMana -= 1;
            if (CurrentMana < 0)
            {
                CurrentMana = 0;
            }
        }
        _rigidbody.velocity = new Vector2(_horizontalDirection * _speed, _rigidbody.velocity.y);

    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundChecker.position,_groundCheckerRadius);
        Gizmos.color=Color.green;
        Gizmos.DrawWireSphere(_headChecker.position,_headCheckerRadius);
        Gizmos.color=Color.red;
        Gizmos.DrawWireCube(_swordAttackPoint.position,new Vector3(_swordAttackRadius,_swordAttackRadius,0));
    }

    private void Attack()
    {
        Collider2D[] targets = Physics2D.OverlapBoxAll(_swordAttackPoint.position,
            new Vector2(_swordAttackRadius, _swordAttackRadius), _whatIsEnemy);
        foreach (var target in targets)
        {
            RangedEnemy rangedEnemy = target.GetComponent<RangedEnemy>();
            if (rangedEnemy != null)
            {
                rangedEnemy.TakeDamage(_swordDamage);
            }

            Jumper jumper = target.GetComponent<Jumper>();
            if (jumper != null)
            {
                jumper.TakeDamage(_swordDamage);
            }
            Crab crab = target.GetComponent<Crab>();
            if (crab != null)
            {
                crab.TakeDamage(_swordDamage);
            }
            FlyEnemy flyEnemy = target.GetComponent<FlyEnemy>();
            if (flyEnemy != null)
            {
                flyEnemy.TakeDamage(_swordDamage);
            }
        }
        _needToAttack = false;  
    }

    private void Cast()
    {
        RaycastHit2D[] hits =
            Physics2D.RaycastAll(_skillCastPoint.position, transform.right, _skillLength, _whatIsEnemy);
        foreach(var hit in hits)
        {
            RangedEnemy target = hit.collider.GetComponent<RangedEnemy>();
            if (target != null)
            {
                target.TakeDamage(_skillDamage);
            }
            Jumper jumper = hit.collider.GetComponent<Jumper>();
            if (jumper != null)
            {
                jumper.TakeDamage(_swordDamage);
            }
            Crab crab = hit.collider.GetComponent<Crab>();
            if (crab != null)
            {
                crab.TakeDamage(_swordDamage);
            }
            FlyEnemy flyEnemy = hit.collider.GetComponent<FlyEnemy>();
            if (flyEnemy != null)
            {
                flyEnemy.TakeDamage(_swordDamage);
            }
        }
        _castLine.SetPosition(0,_skillCastPoint.position);
        _castLine.SetPosition(1,_skillCastPoint.position+transform.right*_skillLength);
        _castLine.enabled = true;
        _needToCast = false;
        Invoke(nameof(DisableLine),0.1f);
    }

    private void DisableLine()
    {
        _castLine.enabled = false;
    }
    public void AddHp(int hpPoints)
    {
        int missingHp = _maxHp - CurrentHp;
        int pointToAdd = missingHp > hpPoints ? hpPoints : missingHp;
        StartCoroutine(RestoreHp(pointToAdd));
    }

    private IEnumerator RestoreHp(int pointToAdd)
    {
        int hp = CurrentHp;
        while (pointToAdd !=0)
        {
            pointToAdd--;
            CurrentHp++;
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void AddMana(int ManaPoints)
    {
        int missingMana = _maxMana - CurrentMana;
        int manaToAdd = missingMana > ManaPoints ? ManaPoints : missingMana;
        StartCoroutine(RestoreMana(manaToAdd));
    }
    private IEnumerator RestoreMana(int manaToAdd)
    {
        int mana = CurrentMana;
        while (manaToAdd !=0)
        {
            manaToAdd--;
            CurrentMana++;
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void AddArmor(int ArmorPoints)
    {
        int missingArmor = _maxArmor - CurrentArmor;
        int armorToAdd = missingArmor > ArmorPoints ? ArmorPoints : missingArmor;
        StartCoroutine(RestoreArmor(armorToAdd));
    }
    private IEnumerator RestoreArmor(int armorToAdd)
    {
        int armor = CurrentArmor;
        while (armorToAdd !=0)
        {
            armorToAdd--;
            CurrentArmor++;
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void TakeDamage(int damage , float pushPower=0 , float enemyPosX=0)
    {
        if (_animator.GetBool(_hurtAnimationKey))
        {
            return;
        }
        CurrentArmor -= damage;
        if (CurrentArmor <= 0)
        {
            CurrentHp -= damage;
            if (_currentHp <= 0)
            {
                Debug.Log(message: " Died ");
                gameObject.SetActive(false);
                Invoke(nameof(ReloadScene), 1f);
            }
        }

        if (pushPower != 0 && Time.time - _lastPushTime > 0.5f)
        {
            _lastPushTime = Time.time;
            int direction = transform.position.x > enemyPosX ? 1 : -1;
            _rigidbody.AddForce(new Vector2(direction*pushPower/2,pushPower));
            _animator.SetBool(_hurtAnimationKey,true);
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
