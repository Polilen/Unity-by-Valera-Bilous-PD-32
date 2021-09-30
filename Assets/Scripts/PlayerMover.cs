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
    [FormerlySerializedAs("_coinAmount")]
    [Header(("UI"))] 
    [SerializeField] private TMP_Text _coinAmountText; 
    [SerializeField] private Slider _hpBar;
    [SerializeField] private Slider _manaBar;
    [SerializeField] private Slider _armorBar;
    private float _horizontalDirection;
    private float _verticalDirection;
    private bool _jump;
    private bool _crawl;
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
        if (Input.GetKey(KeyCode.U))
        {
            CurrentMana -= 1;
            if (CurrentMana < 0)
            {
                CurrentMana = 0;
            }
        }
        
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundChecker.position,_groundCheckerRadius);
        Gizmos.color=Color.green;
        Gizmos.DrawWireSphere(_headChecker.position,_headCheckerRadius);
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
    public void TakeDamage(int damage)
    {
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
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
