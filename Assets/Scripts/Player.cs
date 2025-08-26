using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _extraFallMultiplier;

    [Header("Dodge / Swap")]
    [SerializeField] private float _dodgeDuration;
    [SerializeField] private float _dodgeSpeedMultiplier;
    [SerializeField] private float _swapDuration;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckDistance;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Stats")]
    [SerializeField] private int _health;
    [SerializeField] private int _ammo;
    [SerializeField] private int _coin;
    [SerializeField] private int _grenadeCount;

    private const int MaxAmmo = 999;
    private const int MaxCoin = 99999;
    private const int MaxHealth = 100;
    private const int MaxGrenadeCount = 4;
    private const float MoveEpsilon = 0.0001f;

    [Header("Weapons & Items")]
    [SerializeField] private GameObject[] _weapons;
    [SerializeField] private bool[] _hasWeapons;
    [SerializeField] private GameObject[] _grenades;

    // State
    private Vector2 _moveInput;
    private Vector3 _moveDirection;
    private Vector3 _dodgeDirection;

    private bool _isWalking;
    private bool _isJumping;
    private bool _shouldJump;
    private bool _isDodging;
    private bool _isSwapping;
    private bool _isAttacking;
    private bool _isAttackHeld;

    private int _currentWeaponId = -1;
    private float _speedMultiplier = 1f;
    private float _nextAttackTime;

    private Animator _animator;
    private Rigidbody _rb;

    private WeaponBase _currentWeapon;
    private GameObject _nearObj;

    private Coroutine _dodgeCo;
    private Coroutine _swapCo;
    private Coroutine _attackCo;

    private static readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int IsJumpingHash = Animator.StringToHash("isJumping");
    private static readonly int DoJumpHash = Animator.StringToHash("doJump");
    private static readonly int DoDodgeHash = Animator.StringToHash("doDodge");
    private static readonly int DoSwapHash = Animator.StringToHash("doSwap");
    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();

        // 방어적으로 GroundCheckPoint 비어있다면 하위에서 찾아보기
        if (_groundCheckPoint == null)
            _groundCheckPoint = transform.Find("GroundCheckPoint");
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

        bool isMoving = _moveDirection.sqrMagnitude > MoveEpsilon;
        if (isMoving)
        {
            transform.forward = _moveDirection;
        }

        float speed = (_isWalking ? _walkSpeed : _runSpeed) * _speedMultiplier;
        Vector3 moveXZ = new Vector3(_moveDirection.x, 0f, _moveDirection.z) * speed;
        _rb.velocity = new Vector3(moveXZ.x, _rb.velocity.y, moveXZ.z);

        _animator.SetBool(IsRunningHash, isMoving);
        _animator.SetBool(IsWalkingHash, _isWalking);
    }
    private void HandleJumpFall()
    {
        bool wasJumping = _isJumping;

        if (_rb.velocity.y < 0f)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (_extraFallMultiplier - 1f) * Time.fixedDeltaTime;

            if (IsGrounded())
            {
                _isJumping = false;
            }
        }
        if (wasJumping != _isJumping)
        {
            _animator.SetBool(IsJumpingHash, _isJumping);
        }

    }
    private bool IsGrounded()
    {
        if (_groundCheckPoint == null) return false;
        // 하강 중일 때만 바닥 감지
        if (_rb.velocity.y > 0f) return false;

        return Physics.Raycast(_groundCheckPoint.position, Vector3.down, _groundCheckDistance, _groundLayer);
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
        if (!context.started || _isJumping || _isDodging || _isSwapping) return;

        bool hasMoveInput = _moveInput.sqrMagnitude > MoveEpsilon;

        if (!hasMoveInput)
        {
            _shouldJump = true;
            _isJumping = true;
            _animator.SetTrigger(DoJumpHash);
            _animator.SetBool(IsJumpingHash, _isJumping);
        }
        else
        {
            _dodgeDirection = _moveDirection;
            _animator.SetTrigger(DoDodgeHash);
            RestartRoutine(ref _dodgeCo, DodgeRoutine(_dodgeDuration, _dodgeSpeedMultiplier));
        }
    }
    public void OnSwapWeapon(InputAction.CallbackContext context)
    {
        if (!context.started) { return; }
        if (_isJumping || _isDodging) { return; }
            
        int newWeaponId = Mathf.RoundToInt(context.ReadValue<float>());

        if (_weapons == null || newWeaponId < 0 || newWeaponId >= _weapons.Length) return;//////////////
        if (_hasWeapons == null || newWeaponId >= _hasWeapons.Length || !_hasWeapons[newWeaponId]) return;//////////////////
        if (newWeaponId == _currentWeaponId) return;

        if (_currentWeapon != null)
        {
            _currentWeapon.gameObject.SetActive(false);
        }

        _currentWeaponId = newWeaponId;

        //1
        _currentWeapon = _weapons[_currentWeaponId].GetComponent<WeaponBase>();
        _currentWeapon.gameObject.SetActive(true);

        _animator.SetTrigger(DoSwapHash);

        if (_swapCo != null)
        {
            StopCoroutine(_swapCo);
        }
        _swapCo = StartCoroutine(SwapRoutine(0.5f));

        //2
        GameObject go = _weapons[_currentWeaponId];
        if (go != null && go.TryGetComponent(out WeaponBase weapon))
        {
            _currentWeapon = weapon;
            _currentWeapon.gameObject.SetActive(true);
            _animator.SetTrigger(DoSwapHash);
            RestartRoutine(ref _swapCo, SwapRoutine(_swapDuration));
        }
    }
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (!context.started || _nearObj == null || _isJumping || _isDodging) return;

        if (_nearObj.CompareTag(Tags.Weapon))
        {
            if (_nearObj.TryGetComponent(out Item item))
            {
                if (_hasWeapons != null && item.Value >= 0 && item.Value < _hasWeapons.Length)/////////////
                    _hasWeapons[item.Value] = true;
            }
            Destroy(_nearObj);
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isAttackHeld = true;
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
        float _prev = _speedMultiplier;
        _speedMultiplier = multiplier;

        yield return new WaitForSeconds(duration);

        _speedMultiplier = _prev;
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
        if (!other.CompareTag(Tags.Item)) {  return; }

        if (!other.TryGetComponent(out Item item)) return;

        switch (item.Type)
        {
            case ItemType.Ammo:
                _ammo = Mathf.Min(_ammo + item.Value, MaxAmmo);
                break;
            case ItemType.Coin:
                _coin = Mathf.Min(_coin + item.Value, MaxCoin);
                break;
            case ItemType.Heart:
                _health = Mathf.Min(_health + item.Value, MaxHealth);
                break;
            case ItemType.Grenade:
                //if (_grenadeCount == MaxGrenadeCount)
                //    return;
                //_grenades[_grenadeCount].SetActive(true);
                //_grenadeCount = _grenadeCount + item.Value;


                if (_grenades != null && _grenades.Length > 0)//////
                {
                    int before = _grenadeCount;
                    int after = Mathf.Clamp(_grenadeCount + item.Value, 0, MaxGrenadeCount);/////

                    // UI 토글: 새로 늘어난 슬롯만 활성화
                    for (int i = before; i < after && i < _grenades.Length; i++)//////
                    {
                        if (_grenades[i] != null)
                        {
                            _grenades[i].SetActive(true);
                        }
                    }

                    _grenadeCount = after;
                }
                break;
        }
        Destroy(other.gameObject);
        
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
    // --- Utils ---
    private void RestartRoutine(ref Coroutine coField, IEnumerator routine)
    {
        if (coField != null) StopCoroutine(coField);
        coField = StartCoroutine(routine);
    }
}
