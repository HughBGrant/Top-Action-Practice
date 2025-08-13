using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float jumpPower;

    [SerializeField] private GameObject[] weapons;
    [SerializeField] private bool[] hasWeapons;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private Vector3 dodgeDirection;
    [SerializeField] private int currentWeaponId;
    [SerializeField] private GameObject currentWeapon;

    private bool isRunning;
    private bool isWalking;
    private bool isJumping;
    private bool isJumpRequested;
    private bool isDodging;
    private bool isSwitching;

    private float speedMultiplier = 1f;

    private static readonly int isRunningHash = Animator.StringToHash("isRunning");
    private static readonly int isWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int isJumpingHash = Animator.StringToHash("isJumping");
    private static readonly int doJumpHash = Animator.StringToHash("doJump");
    private static readonly int doDodgeHash = Animator.StringToHash("doDodge");
    private static readonly int doSwitchHash = Animator.StringToHash("doSwitch");
    private static readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private Animator animator;
    private Rigidbody rb;
    private GameObject nearObj;

    private Coroutine dodgeCo;
    private Coroutine switchCo;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if (isJumpRequested)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
            isJumpRequested = false;
        }
        moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        currentSpeed = (isWalking ? walkSpeed : runSpeed) * speedMultiplier;

        if (isDodging)
        {
            moveDirection = dodgeDirection;
        }
        rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);
        transform.LookAt(transform.position + moveDirection);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            animator.SetBool(isJumpingHash, isJumping);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObj = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObj = null;
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        animator.SetBool(isRunningHash, isRunning);
    }
    public void OnWalk(InputAction.CallbackContext context)
    {
        isWalking = context.ReadValueAsButton();
        animator.SetBool(isWalkingHash, isWalking);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && !isJumping && !isDodging)
        {
            if (!isRunning)
            {
                isJumpRequested = true;
                animator.SetTrigger(doJumpHash);

                isJumping = true;
                animator.SetBool(isJumpingHash, isJumping);
            }
            else
            {
                dodgeDirection = moveDirection;
                animator.SetTrigger(doDodgeHash);

                if (dodgeCo != null)
                {
                    StopCoroutine(dodgeCo);
                }
                dodgeCo = StartCoroutine(DodgeRoutine(0.5f, 2f));
            }
        }
    }
    public void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if (context.started && !isJumping && !isDodging)
        {
            if (currentWeapon != null)
            {
                currentWeapon.SetActive(false);
            }
            currentWeaponId = Mathf.RoundToInt(context.ReadValue<float>());


            currentWeapon = weapons[currentWeaponId];
            
            currentWeapon.SetActive(true);

            animator.SetTrigger(doSwitchHash);

            isSwitching = true;
        }
    }
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.started && nearObj != null && !isJumping && !isDodging)
        {
            if (nearObj.tag == "Weapon")
            {
                Item item = nearObj.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObj);
            }
        }
    }
    private IEnumerator DodgeRoutine(float duration, float multiplier)
    {
        isDodging = true;

        // 속도 배수 적용 (겹치는 상황 방지 위해 기존 배수 저장)
        float prevMultiplier = speedMultiplier;
        speedMultiplier = multiplier;

        float t = 0f;
        while (t < duration)
        {
            t += Time.fixedDeltaTime; // 물리 프레임 기준으로 진행
            yield return waitForFixedUpdate;
        }

        // 원복
        speedMultiplier = prevMultiplier;
        isDodging = false;
        dodgeCo = null;
        //moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
    }

    void SwitchEnd()
    {
        currentSpeed *= 0.5f;
        isDodging = false;
    }
}
