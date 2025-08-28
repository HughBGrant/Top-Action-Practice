using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private float fallGravityMultiplier;

    [Header("Dodge / Swap / Reload")]
    [SerializeField]
    private float dodgeDuration;
    [SerializeField]
    private float dodgeSpeedMultiplier;
    [SerializeField]
    private float swapDuration;
    [SerializeField]
    private float reloadDuration;

    [Header("Ground Check")]
    [SerializeField]
    private Transform groundCheckPoint;
    [SerializeField]
    private float groundCheckDistance;
    [SerializeField]
    private LayerMask groundLayer;

    [Header("Stats")]
    [SerializeField]
    private int health;
    [SerializeField]
    private int ammo;
    [SerializeField]
    private int coin;
    [SerializeField]
    private int grenadeCount;

    [Header("Weapons & Items")]
    [SerializeField]
    private GameObject[] weapons;
    [SerializeField]
    private bool[] hasWeapons;
    [SerializeField]
    private GameObject[] grenades;

    // State
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Vector3 dodgeDirection;

    private bool isWalking;
    private bool isJumping;
    private bool shouldJump;
    private bool isDodging;
    private bool isSwapping;
    private bool isAttacking;
    private bool isAttackHeld;
    private bool isReloading;

    private int currentWeaponId = -1;
    private float nextAttackTime;

    private Animator animator;
    private Rigidbody rb;

    private WeaponBase currentWeapon;
    private GameObject nearObj;

    private Coroutine dodgeCo;
    private Coroutine swapCo;
    private Coroutine attackCo;
    private Coroutine reloadCo;
    private const int MaxAmmo = 999;
    private const int MaxCoin = 99999;
    private const int MaxHealth = 100;
    private const int MaxGrenadeCount = 4;
    private const float MoveEpsilon = 0.0001f;

    private static readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int IsJumpingHash = Animator.StringToHash("isJumping");
    private static readonly int DoJumpHash = Animator.StringToHash("doJump");
    private static readonly int DoDodgeHash = Animator.StringToHash("doDodge");
    private static readonly int DoSwapHash = Animator.StringToHash("doSwap");
    private static readonly int DoReloadHash = Animator.StringToHash("doReload");
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        // 방어적으로 GroundCheckPoint 비어있다면 하위에서 찾아보기
        if (groundCheckPoint == null)
            groundCheckPoint = transform.Find("GroundCheckPoint");
    }
    void FixedUpdate()
    {
        HandleMovement();
        HandleJumpFall();
    }
    private void HandleMovement()
    {
        if (shouldJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
            shouldJump = false;
        }

        moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        if (isDodging)
        {
            moveDirection = dodgeDirection;
        }
        if (isSwapping || isAttacking || isReloading)
        {
            moveDirection = Vector3.zero;
        }

        float speed = (isWalking ? walkSpeed : runSpeed) * (isDodging ? dodgeSpeedMultiplier : 1f);
        Vector3 moveXZ = new Vector3(moveDirection.x, 0f, moveDirection.z) * speed;
        rb.velocity = new Vector3(moveXZ.x, rb.velocity.y, moveXZ.z);

        bool isRunning = moveDirection.sqrMagnitude > MoveEpsilon;
        if (isRunning)
        {
            transform.forward = moveDirection;
        }
        animator.SetBool(IsRunningHash, isRunning);
        animator.SetBool(IsWalkingHash, isWalking);
    }
    private void HandleJumpFall()
    {
        bool wasJumping = isJumping;

        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1f) * Time.fixedDeltaTime;

            if (IsGrounded())
            {
                isJumping = false;
            }
        }
        if (wasJumping != isJumping)
        {
            animator.SetBool(IsJumpingHash, isJumping);
        }
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnWalk(InputAction.CallbackContext context)
    {
        isWalking = context.ReadValueAsButton();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started || isJumping || isDodging || isSwapping) { return; }

        bool hasMoveInput = moveInput.sqrMagnitude > MoveEpsilon;

        if (!hasMoveInput)
        {
            shouldJump = true;
            isJumping = true;
            animator.SetTrigger(DoJumpHash);
            animator.SetBool(IsJumpingHash, isJumping);
        }
        else
        {
            dodgeDirection = moveDirection;
            animator.SetTrigger(DoDodgeHash);
            RestartRoutine(ref dodgeCo, DodgeRoutine());
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentWeapon == null || isDodging || isSwapping || isAttacking) { return; }
            isAttackHeld = true;

            //if (attackCo == null)
            //{
            //    attackCo = StartCoroutine(AttackRoutine());
            //}
            RestartRoutine(ref attackCo, AttackRoutine());
        }
        else if (context.canceled)
        {
            isAttackHeld = false;
        }
    }
    public void OnReload(InputAction.CallbackContext context)
    {
        if (!context.started || isJumping || isDodging || isSwapping || isAttacking) { return; }
        if (currentWeapon == null || currentWeapon is MeleeWeapon || ammo == 0) { return; }/////////

        animator.SetTrigger(DoReloadHash);
        isReloading = true;
        RestartRoutine(ref reloadCo, ReloadRoutine());
    }
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (!context.started || isJumping || isDodging) { return; }
        if (nearObj == null) { return; }

        if (nearObj.CompareTag(Tags.Weapon))
        {
            if (nearObj.TryGetComponent(out Item item))
            {
                if (hasWeapons != null && item.Value >= 0 && item.Value < hasWeapons.Length)
                {
                    hasWeapons[item.Value] = true;
                }
            }
            Destroy(nearObj);
        }
    }
    public void OnSwapWeapon(InputAction.CallbackContext context)
    {
        if (!context.started) { return; }
        if (isJumping || isDodging) { return; }
        if (weapons == null || hasWeapons == null) { return; }

        int newWeaponId = Mathf.RoundToInt(context.ReadValue<float>());

        if (newWeaponId < 0 || newWeaponId >= weapons.Length || newWeaponId >= hasWeapons.Length || newWeaponId == currentWeaponId || !hasWeapons[newWeaponId]) { return; }

        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }
        currentWeaponId = newWeaponId;

        GameObject go = weapons[currentWeaponId];
        if (go != null && go.TryGetComponent(out WeaponBase weapon))
        {
            currentWeapon = weapon;
            currentWeapon.gameObject.SetActive(true);
            animator.SetTrigger(DoSwapHash);
            RestartRoutine(ref swapCo, SwapRoutine());
        }
    }
    private IEnumerator DodgeRoutine()
    {
        isDodging = true;
        yield return new WaitForSeconds(dodgeDuration);

        isDodging = false;
        dodgeCo = null;
    }
    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadDuration);

        int newAmmo = Mathf.Min(ammo, currentWeapon.MaxMagazine - currentWeapon.CurrentMagazine);
        ammo -= newAmmo;
        Debug.Log(currentWeapon.CurrentMagazine);
        currentWeapon.CurrentMagazine += newAmmo;
        Debug.Log(currentWeapon.CurrentMagazine);

        isReloading = false;
        reloadCo = null;
    }
    private IEnumerator SwapRoutine()
    {
        isSwapping = true;
        yield return new WaitForSeconds(swapDuration);

        isSwapping = false;
        swapCo = null;
    }
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        while (isAttackHeld)
        {
            if (Time.time > nextAttackTime)
            {
                animator.SetTrigger(currentWeapon.DoAttackHash);//////////////
                currentWeapon.Use();
                nextAttackTime = Time.time + currentWeapon.AttackSpeed;
            }
            yield return null;
        }
        yield return new WaitForSeconds(currentWeapon.AttackSpeed);
        isAttacking = false;
        attackCo = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.Item)) {  return; }

        if (!other.TryGetComponent(out Item item)) { return; }

        switch (item.Type)
        {
            case ItemType.Ammo:
                ammo = Mathf.Min(ammo + item.Value, MaxAmmo);
                break;
            case ItemType.Coin:
                coin = Mathf.Min(coin + item.Value, MaxCoin);
                break;
            case ItemType.Heart:
                health = Mathf.Min(health + item.Value, MaxHealth);
                break;
            case ItemType.Grenade:
                if (grenades != null && grenades.Length > 0)
                {
                    int before = grenadeCount;
                    int after = Mathf.Clamp(grenadeCount + item.Value, 0, MaxGrenadeCount);

                    // UI 토글: 새로 늘어난 슬롯만 활성화
                    for (int i = before; i < after && i < grenades.Length; i++)
                    {
                        if (grenades[i] != null)
                        {
                            grenades[i].SetActive(true);
                        }
                    }
                    grenadeCount = after;
                }
                break;
        }
        Destroy(other.gameObject);
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.Weapon))
        {
            nearObj = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Weapon))
        {
            nearObj = null;
        }
    }
    // --- Utils ---
    private void RestartRoutine(ref Coroutine coField, IEnumerator routine)
    {
        if (coField != null)
        {
            StopCoroutine(coField);
        }
        coField = StartCoroutine(routine);
    }
    private bool IsGrounded()
    {
        if (groundCheckPoint == null)
        {
            return false;
        }
        // 하강 중일 때만 바닥 감지
        if (rb.velocity.y > 0f)
        {
            return false;
        }
        return Physics.Raycast(groundCheckPoint.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}
