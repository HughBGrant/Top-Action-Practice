using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _extraFallMultiplier = 2f;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckDistance;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Stats")]
    [SerializeField] private int _health;
    [SerializeField] private int _ammo;
    [SerializeField] private int _coin;
    [SerializeField] private int _grenadeCount;

    private const int _maxAmmo = 999;
    private const int _maxCoin = 99999;
    private const int _maxHealth = 100;
    private const int _maxGrenadeCount = 4;

    [Header("Weapons & Items")]
    [SerializeField] private GameObject[] _weapons;
    [SerializeField] private bool[] _hasWeapons;
    [SerializeField] private GameObject[] _grenades;

    private int _currentWeaponId = -1;
    private WeaponBase _currentWeapon;
    private GameObject _nearObj;

    [Header("State Flags")]
    [SerializeField] private Vector2 _moveInput;
    [SerializeField] private Vector3 _moveDirection;
    [SerializeField] private Vector3 _dodgeDirection;

    private bool _isRunning;
    private bool _isWalking;
    private bool _isJumping;
    private bool _shouldJump;
    private bool _isDodging;
    private bool _isSwapping;
    private bool _isAttacking;
    private bool _isAttackHeld;

    private float _speedMultiplier = 1f;
    private float _nextAttackTime;

    private Animator _animator;
    private Rigidbody _rb;

    private Coroutine _dodgeCo;
    private Coroutine _swapCo;
    private Coroutine _attackCo;

    private static readonly int _isRunningHash = Animator.StringToHash("isRunning");
    private static readonly int _isWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int _isJumpingHash = Animator.StringToHash("isJumping");
    private static readonly int _doJumpHash = Animator.StringToHash("doJump");
    private static readonly int _doDodgeHash = Animator.StringToHash("doDodge");
    private static readonly int _doSwapHash = Animator.StringToHash("doSwap");
    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        HandleMovement();
        HandleJumpFall();
    }
    private void HandleMovement()
    {
        if (_shouldJump)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpPower, _rb.velocity.z);
            _shouldJump = false;
        }

        _moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);
        if (_isDodging)
        {
            _moveDirection = _dodgeDirection;
        }
        if (_isSwapping || _isAttacking)
        {
            _moveDirection = Vector3.zero;
        }

        float speed = (_isWalking ? _walkSpeed : _runSpeed) * _speedMultiplier;
        Vector3 moveVector = new Vector3(_moveDirection.x, 0f, _moveDirection.z) * speed;
        _rb.velocity = new Vector3(moveVector.x, _rb.velocity.y, moveVector.z);

        if (_moveDirection.sqrMagnitude > 0.0001f)
        {
            transform.LookAt(transform.position + _moveDirection);
        }
        _isRunning = _moveDirection.sqrMagnitude > 0.0001f;
        _animator.SetBool(_isRunningHash, _isRunning);
        _animator.SetBool(_isWalkingHash, _isWalking);


    }
    private void HandleJumpFall()
    {
        bool wasJumping = _isJumping;
        if (_rb.velocity.y < 0f)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (_extraFallMultiplier - 1f) * Time.fixedDeltaTime;

            if (Physics.Raycast(_groundCheckPoint.position, Vector3.down, _groundCheckDistance, _groundLayer))
            {
                _isJumping = false;
            }
        }
        if (wasJumping != _isJumping)
        {
            _animator.SetBool(_isJumpingHash, _isJumping);
        }

    }
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
    public void OnWalk(InputAction.CallbackContext context)
    {
        _isWalking = context.ReadValueAsButton();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started) { return; }
        if (_isJumping || _isDodging || _isSwapping) { return; }
        if (!_isRunning)
        {
            _shouldJump = true;
            _animator.SetTrigger(_doJumpHash);

            _isJumping = true;
            _animator.SetBool(_isJumpingHash, _isJumping);
        }
        else
        {
            _dodgeDirection = _moveDirection;
            _animator.SetTrigger(_doDodgeHash);

            if (_dodgeCo != null)
            {
                StopCoroutine(_dodgeCo);
            }
            _dodgeCo = StartCoroutine(DodgeRoutine(0.5f, 2f));
        }
    }
    public void OnSwapWeapon(InputAction.CallbackContext context)
    {
        if (!context.started) { return; }
        if (_isJumping || _isDodging) { return; }
            
        int newWeaponId = Mathf.RoundToInt(context.ReadValue<float>());
        if (!_hasWeapons[newWeaponId] || newWeaponId == _currentWeaponId) { return; }

        if (_currentWeapon != null)
        {
            _currentWeapon.gameObject.SetActive(false);
        }

        _currentWeaponId = newWeaponId;
        _currentWeapon = _weapons[_currentWeaponId].GetComponent<WeaponBase>();
        _currentWeapon.gameObject.SetActive(true);

        _animator.SetTrigger(_doSwapHash);

        if (_swapCo != null)
        {
            StopCoroutine(_swapCo);
        }
        _swapCo = StartCoroutine(SwapRoutine(0.5f));
        
    }
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (!context.started) { return; }
        if (_nearObj == null) { return; }
        if (_isJumping || _isDodging) { return; }

        if (_nearObj.CompareTag(Tags.Weapon))
        {
            Item item = _nearObj.GetComponent<Item>();
            _hasWeapons[item.Value] = true;
            Destroy(_nearObj);
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isAttackHeld = true;
            // 이미 루틴이 돌고 있지 않으면 시작
            if (_attackCo == null)
            {
                _attackCo = StartCoroutine(AttackRoutine());
            }
        }
        else if (context.canceled)
        {
            _isAttackHeld = false;
        }
    }
    private IEnumerator DodgeRoutine(float duration, float multiplier)
    {
        _isDodging = true;
        // 속도 배수 적용 (겹치는 상황 방지 위해 기존 배수 저장)
        float _prevMultiplier = _speedMultiplier;
        _speedMultiplier = multiplier;

        yield return new WaitForSeconds(duration);

        _speedMultiplier = _prevMultiplier;
        _isDodging = false;
        _dodgeCo = null;
    }
    private IEnumerator SwapRoutine(float duration)
    {
        _isSwapping = true;

        yield return new WaitForSeconds(duration);
        _isSwapping = false;
        _swapCo = null;
    }
    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        while (_isAttackHeld)
        {
            if (_currentWeapon != null && !_isDodging && !_isSwapping && Time.time > _nextAttackTime)
            {
                _animator.SetTrigger(_currentWeapon.doAttackHash);
                _currentWeapon.Use();
                _nextAttackTime = Time.time + _currentWeapon.AttackSpeed;
                
            }
            yield return null;
        }
        _isAttacking = false;
        _attackCo = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Item))
        {
            Item item = other.GetComponent<Item>();
            switch (item.Type)
            {
                case ItemType.Ammo:
                    _ammo = Mathf.Min(_ammo + item.Value, _maxAmmo);
                    break;
                case ItemType.Coin:
                    _coin = Mathf.Min(_coin + item.Value, _maxCoin);
                    break;
                case ItemType.Heart:
                    _health = Mathf.Min(_health + item.Value, _maxHealth);
                    break;
                case ItemType.Grenade:
                    if (_grenadeCount == _maxGrenadeCount)
                        return;
                    _grenades[_grenadeCount].SetActive(true);
                    _grenadeCount = _grenadeCount + item.Value;
                    break;
            }
            Destroy(other.gameObject);
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.Weapon))
        {
            _nearObj = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Weapon))
        {
            _nearObj = null;
        }
    }
}
