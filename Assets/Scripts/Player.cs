using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{
    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DodgeHash = Animator.StringToHash("Dodge");
    private static readonly int SwapHash = Animator.StringToHash("Swap");
    private static readonly int ReloadHash = Animator.StringToHash("Reload");

    private enum WeaponSlot { None = -1, Hammer, HandGun, SubMachineGun }

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

    [Header("재화, 능력치")]
    [SerializeField]
    private int currentHealth;
    [SerializeField]
    private int ammo;
    [SerializeField]
    private int coin;
    [SerializeField]
    private int grenadeCount;

    //private float[] stats = new float[(int)StatType.Count];
    //public float this[StatType e] { get => stats[(int)e]; set => stats[(int)e] = value; }

    [Header("장착")]
    [SerializeField]
    private GameObject[] belongingWeapons;
    [SerializeField]
    private bool[] hasWeapons;
    [SerializeField]
    private GameObject[] belongingGrenades;
    [SerializeField]
    private GrenadeThrower grenadePrefab;

    // 움직임
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Vector3 dodgeDirection;

    // 상태
    private bool isWalking;
    private bool isJumping;
    private bool shouldJump;
    private bool isDodging;
    private bool isSwapping;
    private bool isAttacking;
    private bool isAttackHeld;
    private bool isReloading;
    private bool isInTouchWall;
    private bool isTakingDamage;

    private Animator animator;
    private Rigidbody rigid;
    private MeshRenderer[] meshs;

    private WeaponBase currentWeapon;
    private WeaponSlot currentWeaponId = WeaponSlot.None;
    private GameObject nearObject;
    private Camera cam;

    private Coroutine dodgeCo;
    private Coroutine swapCo;
    private Coroutine attackCo;
    private Coroutine reloadCo;
    private Coroutine hitCo;

    private const int AmmoCap = 999;
    private const int CoinCap = 99999;
    private const int HealthCap = 100;
    private const int GrenadeCountCap = 4;
    private const float MoveEpsilon = 0.0001f;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        cam = Camera.main;
    }
    void FixedUpdate()
    {
        HandleMovement();
        HandleJumpFall();
    }
    private void HandleMovement()
    {
        //예약된 점프 실행
        if (shouldJump)
        {
            rigid.velocity = new Vector3(rigid.velocity.x, jumpPower, rigid.velocity.z);
            shouldJump = false;
        }
        //이동 준비
        moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        if (isDodging)
        {
            moveDirection = dodgeDirection;
        }
        if (isSwapping || isAttacking || isReloading)
        {
            moveDirection = Vector3.zero;
        }

        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isInTouchWall = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));

        float speed = (isInTouchWall ? 1f : 1f) * (isWalking ? walkSpeed : runSpeed) * (isDodging ? dodgeSpeedMultiplier : 1f);
        //최종 벡터
        Vector3 moveXZ = new Vector3(moveDirection.x, 0f, moveDirection.z) * speed;

        rigid.velocity = new Vector3(moveXZ.x, rigid.velocity.y, moveXZ.z);
        //달리는 방향
        bool isRunning = moveDirection.sqrMagnitude > MoveEpsilon;
        if (isRunning)
        {
            transform.forward = moveDirection;
        }
        //공격시 방향
        if (isAttacking)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Vector3 nextVec = hit.point - transform.position;
                nextVec.y = 0f;
                transform.forward = nextVec;
            }
        }
        //애니메이션
        animator.SetBool(IsRunningHash, isRunning);
        animator.SetBool(IsWalkingHash, isWalking);
    }
    private void HandleJumpFall()
    {
        //바닥 착지
        bool wasJumping = isJumping;

        if (rigid.velocity.y < 0f)
        {
            rigid.velocity += Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1f) * Time.fixedDeltaTime;

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
            animator.SetTrigger(JumpHash);
            animator.SetBool(IsJumpingHash, isJumping);
        }
        else
        {
            dodgeDirection = moveDirection;
            animator.SetTrigger(DodgeHash);
            RestartRoutine(ref dodgeCo, PerformDodge());
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentWeapon == null || isDodging || isSwapping || isAttacking) { return; }
            isAttackHeld = true;

            RestartRoutine(ref attackCo, HandleAttack());

        }
        else if (context.canceled)
        {
            isAttackHeld = false;
        }
    }
    public void OnReload(InputAction.CallbackContext context)
    {
        if (!context.started || isJumping || isDodging || isSwapping || isAttacking) { return; }
        if (currentWeapon == null || currentWeapon is MeleeWeapon || ammo == 0) { return; }

        animator.SetTrigger(ReloadHash);
        isReloading = true;
        RestartRoutine(ref reloadCo, ReloadBullet());
    }
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (!context.started || isJumping || isDodging) { return; }
        if (nearObject == null) { return; }

        if (nearObject.CompareTag(Tag.Weapon))
        {
            if (nearObject.TryGetComponent(out Item item) && hasWeapons != null && item.Value >= 0 && item.Value < hasWeapons.Length)
            {
                hasWeapons[item.Value] = true;
            }
            Destroy(nearObject);
        }
    }
    public void OnSwapWeapon(InputAction.CallbackContext context)
    {
        if (!context.started) { return; }
        if (isJumping || isDodging) { return; }
        if (belongingWeapons == null || hasWeapons == null) { return; }

        int newWeaponId = Mathf.RoundToInt(context.ReadValue<float>());

        if (newWeaponId < 0 || newWeaponId >= belongingWeapons.Length || newWeaponId >= hasWeapons.Length || newWeaponId == (int)currentWeaponId || !hasWeapons[newWeaponId]) { return; }

        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }
        currentWeaponId = (WeaponSlot)newWeaponId;

        GameObject go = belongingWeapons[(int)currentWeaponId];
        if (go != null && go.TryGetComponent(out WeaponBase weapon))
        {
            currentWeapon = weapon;
            currentWeapon.gameObject.SetActive(true);
            animator.SetTrigger(SwapHash);
            RestartRoutine(ref swapCo, SwapWeapon());
        }
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (!context.started) { return; }

        if (grenadeCount <= 0) { return; }

        if (isReloading || isSwapping) { return; }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Vector3 nextVec = hit.point - transform.position;
            nextVec.y = 10;

            GrenadeThrower grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
            grenade.Rigid.AddForce(nextVec, ForceMode.Impulse);
            grenade.Rigid.AddTorque(Vector3.back * 10, ForceMode.Impulse);
            grenadeCount--;
            belongingGrenades[grenadeCount].SetActive(false);
        }
    }
    private IEnumerator PerformDodge()
    {
        isDodging = true;
        yield return YieldCache.WaitForSeconds(dodgeDuration);

        isDodging = false;
        dodgeCo = null;
    }
    private IEnumerator ReloadBullet()
    {
        isReloading = true;
        yield return YieldCache.WaitForSeconds(reloadDuration);

        int newAmmo = Mathf.Min(ammo, currentWeapon.MaxMagazine - currentWeapon.CurrentMagazine);
        ammo -= newAmmo;
        currentWeapon.CurrentMagazine += newAmmo;

        isReloading = false;
        reloadCo = null;
    }
    private IEnumerator SwapWeapon()
    {
        isSwapping = true;
        yield return YieldCache.WaitForSeconds(swapDuration);

        isSwapping = false;
        swapCo = null;
    }
    private IEnumerator HandleAttack()
    {
        isAttacking = true;
        float nextAttackTime = 0;

        while (isAttackHeld)
        {
            if (Time.time > nextAttackTime)
            {
                animator.SetTrigger(currentWeapon.AttackHash);
                currentWeapon.Use();
                nextAttackTime = Time.time + currentWeapon.AttackSpeed;
            }
            yield return null;
        }
        yield return YieldCache.WaitForSeconds(currentWeapon.AttackSpeed);
        isAttacking = false;
        attackCo = null;
    }
    public void TakeDamage(int damage, Vector3 hitPoint, bool isHitGrenade = false)
    {
        if (isTakingDamage) { return; }

        hitCo ??= StartCoroutine(HitFlash());

        currentHealth -= damage;

    }
    private IEnumerator HitFlash()
    {
        isTakingDamage = true;

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }
        yield return YieldCache.WaitForSeconds(1.0f);

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
        isTakingDamage = false;
        hitCo = null;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerIndex.Weapon)
        {
            nearObject = other.gameObject;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerIndex.Item)
        {
            if (!other.TryGetComponent(out Item item)) { return; }

            ApplyItemEffect(item.Type, item.Value);
            Destroy(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerIndex.Weapon)
        {
            nearObject = null;
        }
    }
    // --- Utils ---
    private void RestartRoutine(ref Coroutine co, IEnumerator routine)
    {
        if (co != null)
        {
            StopCoroutine(co);
        }
        co = StartCoroutine(routine);
    }
    private bool IsGrounded()
    {
        if (groundCheckPoint == null)
        {
            return false;
        }
        // 하강 중일 때만 바닥 감지
        if (rigid.velocity.y > 0f)
        {
            return false;
        }
        return Physics.Raycast(groundCheckPoint.position, Vector3.down, groundCheckDistance, groundLayer);
    }
    public void ApplyItemEffect(ItemType type, int value)
    {
        switch (type)
        {
            case ItemType.Ammo:
                ammo = Mathf.Min(ammo + value, AmmoCap);
                break;
            case ItemType.Coin:
                coin = Mathf.Min(coin + value, CoinCap);
                break;
            case ItemType.Heart:
                currentHealth = Mathf.Min(currentHealth + value, HealthCap);
                break;
            case ItemType.Grenade:
                if (belongingGrenades == null || belongingGrenades.Length <= 0) { return; }

                int before = grenadeCount;
                int after = Mathf.Clamp(grenadeCount + value, 0, GrenadeCountCap);

                for (int i = before; i < after && i < belongingGrenades.Length; i++)
                {
                    belongingGrenades[i]?.SetActive(true);
                }
                grenadeCount = after;
                break;
        }
    }
}
