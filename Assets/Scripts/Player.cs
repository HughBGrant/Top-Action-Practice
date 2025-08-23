using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _jumpPower;

    [SerializeField] private GameObject[] _weapons;
    [SerializeField] private bool[] _hasWeapons;
    [SerializeField] private GameObject[] _grenades;
    [SerializeField] private int _grenadeCount;
    [SerializeField] private int _ammo;
    [SerializeField] private int _coin;
    [SerializeField] private int _health = 100;

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

    private int _maxAmmo = 999;
    private int _maxCoin = 99999;
    private int _maxHealth = 100;
    private int _maxGrenadeCount = 4;

    private int _currentWeaponId = -1;
    private float _preDodgeSpeedMultiplier = 1f;
    private float _speedMultiplier = 1f;

    private Animator _animator;
    private Rigidbody _rb;

    private WeaponBase _currentWeapon;
    private GameObject _nearObj;

    private Coroutine _dodgeCo;
    private Coroutine _swapCo;
    private Coroutine _attackCo;

    private static readonly int _isRunningHash = Animator.StringToHash("isRunning");
    private static readonly int _isWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int _isJumpingHash = Animator.StringToHash("isJumping");
    private static readonly int _doJumpHash = Animator.StringToHash("doJump");
    private static readonly int _doDodgeHash = Animator.StringToHash("doDodge");
    private static readonly int _doSwapHash = Animator.StringToHash("doSwap");
    private static readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
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
        _currentSpeed = (_isWalking ? _walkSpeed : _runSpeed) * _speedMultiplier;
        Vector3 moveVector = new Vector3(_moveDirection.x, 0f, _moveDirection.z) * _currentSpeed;
        _rb.velocity = new Vector3(moveVector.x, _rb.velocity.y, moveVector.z);

        if (_moveDirection.sqrMagnitude > 0.0001f)
        {
            transform.LookAt(transform.position + _moveDirection);
        }
        _isRunning = _moveDirection.sqrMagnitude > 0.0001f;
        _animator.SetBool(_isRunningHash, _isRunning);

        _animator.SetBool(_isWalkingHash, _isWalking);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Tags.Ground))
        {
            _isJumping = false;
            _animator.SetBool(_isJumpingHash, _isJumping);
        }
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
                EndDodge();
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
            EndSwap();
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

            // 즉시 정지
            if (_attackCo != null)
            {
                StopCoroutine(_attackCo);
                EndAttack();
            }
        }
    }
    private IEnumerator DodgeRoutine(float duration, float multiplier)
    {
        _isDodging = true;

        // 속도 배수 적용 (겹치는 상황 방지 위해 기존 배수 저장)
        _preDodgeSpeedMultiplier = _speedMultiplier;
        _speedMultiplier = multiplier;

        float t = 0f;
        while (t < duration)
        {
            t += Time.fixedDeltaTime; // 물리 프레임 기준으로 진행
            yield return _waitForFixedUpdate;
        }
        EndDodge();
    }
    private void EndDodge()
    {
        _speedMultiplier = _preDodgeSpeedMultiplier;
        _isDodging = false;
        _dodgeCo = null;
    }
    private IEnumerator SwapRoutine(float duration)
    {
        _isSwapping = true;

        float t = 0f;
        while (t < duration)
        {
            t += Time.fixedDeltaTime; // 물리 프레임 기준으로 진행
            yield return _waitForFixedUpdate;
        }
        EndSwap();
    }
    private void EndSwap()
    {
        _isSwapping = false;
        _swapCo = null;
    }
    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;

        while (_isAttackHeld)
        {
            if (_currentWeapon != null && !_isDodging && !_isSwapping)
            {
                _currentWeapon.Use();
                _animator.SetTrigger(_currentWeapon.doAttackHash);

                yield return new WaitForSeconds(_currentWeapon.AttackSpeed);
                continue;
            }
            yield return null;
        }
        EndAttack();
    }
    private void EndAttack()
    {
        _isAttacking = false;
        _attackCo = null;

    }
}
