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

    [SerializeField] private Vector2 _moveInput;
    [SerializeField] private Vector3 _moveDirection;
    [SerializeField] private Vector3 _dodgeDirection;

    private bool _isRunning;
    private bool _isWalking;
    private bool _isJumping;
    private bool _isJumpRequested;
    private bool _isDodging;
    private bool _isSwapping;

    private int _currentWeaponId = -1;
    private GameObject _currentWeapon;
    private float _speedMultiplier = 1f;
    [SerializeField] private int _ammo;
    [SerializeField] private int _coin;
    [SerializeField] private int _health = 100;

    private int _maxAmmo = 999;
    private int _maxCoin = 99999;
    private int _maxHealth = 100;
    private int _maxGrenadeCount = 4;



    private static readonly int _isRunningHash = Animator.StringToHash("isRunning");
    private static readonly int _isWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int _isJumpingHash = Animator.StringToHash("isJumping");
    private static readonly int _doJumpHash = Animator.StringToHash("doJump");
    private static readonly int _doDodgeHash = Animator.StringToHash("doDodge");
    private static readonly int _doSwapHash = Animator.StringToHash("doSwap");
    private static readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

    private Animator _animator;
    private Rigidbody _rb;
    private GameObject _nearObj;

    private Coroutine _dodgeCo;
    private Coroutine _swapCo;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if (_isJumpRequested)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpPower, _rb.velocity.z);
            _isJumpRequested = false;
        }
        _moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);

        if (_isDodging)
        {
            _moveDirection = _dodgeDirection;
        }
        if (_isSwapping)
        {
            _moveDirection = Vector3.zero;
        }
        _currentSpeed = (_isWalking ? _walkSpeed : _runSpeed) * _speedMultiplier;
        Vector3 moveVector = new Vector3(_moveDirection.x, 0f, _moveDirection.z) * _currentSpeed;
        _rb.velocity = new Vector3(moveVector.x, _rb.velocity.y, moveVector.z);

        if (!_moveDirection.Equals(Vector3.zero))
        {
            transform.LookAt(transform.position + _moveDirection);
        }

        _isRunning = !_moveDirection.Equals(Vector3.zero);
        _animator.SetBool(_isRunningHash, _isRunning);

        _animator.SetBool(_isWalkingHash, _isWalking);

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isJumping = false;
            _animator.SetBool(_isJumpingHash, _isJumping);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
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
        if (other.tag == "Weapon")
        {
            _nearObj = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
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
        if (context.started)
        {
            if (_isJumping || _isDodging || _isSwapping) { return; }
            if (!_isRunning)
            {
                _isJumpRequested = true;
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
    }
    public void OnSwapWeapon(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_isJumping || _isDodging) { return; }
            Debug.Log("0");
            int newWeaponId = Mathf.RoundToInt(context.ReadValue<float>());

            if (!_hasWeapons[newWeaponId] || newWeaponId == _currentWeaponId) { return; }

            Debug.Log("1");
            if (_currentWeapon != null)
            {
                _currentWeapon.SetActive(false);
            }
            _currentWeaponId = newWeaponId;
            _currentWeapon = _weapons[_currentWeaponId];
            
            _currentWeapon.SetActive(true);

            _animator.SetTrigger(_doSwapHash);

            if (_swapCo != null)
            {
                StopCoroutine(_swapCo);
            }
            _swapCo = StartCoroutine(SwapRoutine(0.5f));
        }
    }
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_nearObj == null || _isJumping || _isDodging) { return; }
            if (_nearObj.tag == "Weapon")
            {
                Item item = _nearObj.GetComponent<Item>();
                _hasWeapons[item.Value] = true;

                Destroy(_nearObj);
            }
        }
    }
    private IEnumerator DodgeRoutine(float duration, float multiplier)
    {
        _isDodging = true;
        // 속도 배수 적용 (겹치는 상황 방지 위해 기존 배수 저장)
        float prevMultiplier = _speedMultiplier;
        _speedMultiplier = multiplier;

        float t = 0f;
        while (t < duration)
        {
            t += Time.fixedDeltaTime; // 물리 프레임 기준으로 진행
            yield return _waitForFixedUpdate;
        }

        _speedMultiplier = prevMultiplier;
        _dodgeCo = null;
        _isDodging = false;
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

        _swapCo = null;
        _isSwapping = false;
    }
}
