using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float jumpPower;

    private bool isRunning;
    private bool isWalking;
    private bool isJumpRequested;
    private bool isJumping;
    private bool isDodging;
    private static readonly int isRunningHash = Animator.StringToHash("isRunning");
    private static readonly int isWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int isJumpingHash = Animator.StringToHash("isJumping");
    private static readonly int doJumpHash = Animator.StringToHash("doJump");
    private static readonly int doDodgeHash = Animator.StringToHash("doDodge");

    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private Vector3 dodgeDirection;

    private Animator animator;
    private Rigidbody rb;
    private GameObject nearObj;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        animator.SetBool(isRunningHash, isRunning);
    }

    void FixedUpdate()
    {
        if (isJumpRequested)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
            isJumpRequested = false;
        }

        currentSpeed = isWalking ? walkSpeed : runSpeed;

        moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        if (isDodging)
        {
            rb.velocity = new Vector3(dodgeDirection.x * currentSpeed, rb.velocity.y, dodgeDirection.z * currentSpeed);
            transform.LookAt(transform.position + dodgeDirection);
        }
        else
        {
            rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);
            transform.LookAt(transform.position + moveDirection);
        }
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
        if (context.performed)
        {
            isRunning = true;
            moveInput = context.ReadValue<Vector2>();
        }
        if (context.canceled)
        {
            isRunning = false;
            moveInput = context.ReadValue<Vector2>();
        }

    }
    public void OnWalk(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isWalking = true;
            animator.SetBool(isWalkingHash, isWalking);
        }
        else if (context.canceled)
        {
            isWalking = false;
            animator.SetBool(isWalkingHash, isWalking);
        }
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
                currentSpeed *= 2;
                isDodging = true;
                animator.SetTrigger(doDodgeHash);

                Invoke("DodgeEnd", 0.5f);///////////
            }
        }
    }
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.started && nearObj != null && isJumping)
        {

        }
    }
    void DodgeEnd()
    {
        currentSpeed *= 0.5f;
        isDodging = false;
    }
}
