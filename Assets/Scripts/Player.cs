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


    private bool isWalking;
    private bool isJumpRequested;
    private bool isGrounded;
    private bool isDodging;
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
        animator.SetBool("isRunning", moveDirection != Vector3.zero);
    }

    void FixedUpdate()
    {
        if (isJumpRequested)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
            isJumpRequested = false;
        }

        moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        if (isDodging)
        {
            moveDirection = dodgeDirection;
        }
        currentSpeed = isWalking ? walkSpeed : runSpeed;

        rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);

        if (moveDirection != Vector3.zero)
        {
            transform.LookAt(transform.position + moveDirection);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            animator.SetBool("isJumping", false);
            isGrounded = true;
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

        if (context.started || context.canceled)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }
    public void OnWalk(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isWalking = true;
            animator.SetBool("isWalking", true);
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded && !isDodging)
        {
            if (moveDirection == Vector3.zero)
            {
                isJumpRequested = true;
                animator.SetTrigger("doJump");

                isGrounded = false;
                animator.SetBool("isJumping", true);
            }
            else
            {
                dodgeDirection = moveDirection;
                currentSpeed *= 2;
                isDodging = true;
                animator.SetTrigger("doDodge");

                Invoke("DodgeEnd", 0.5f);///////////
            }
        }
    }
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.started && nearObj != null && !isGrounded)
        {

        }
    }
    public void OnDodge(InputAction.CallbackContext context)
    {
    }
    void DodgeEnd()
    {
        currentSpeed *= 0.5f;
        isDodging = false;
    }
}
