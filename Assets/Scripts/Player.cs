using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpPower;

    private bool isWalking;
    private bool isGrounded;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private bool jumpQueued; // 점프 명령 대기

    private Animator animator;
    private Rigidbody rb;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        animator.SetBool("isRunning", moveDirection != Vector3.zero);
        animator.SetBool("isWalking", isWalking);
    }

    void FixedUpdate()
    {
        if (jumpQueued)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
            jumpQueued = false;
        }
        moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        float currentSpeed = isWalking ? walkSpeed : runSpeed;

        rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);

        if (moveDirection != Vector3.zero)///////////////////
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
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            //rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            jumpQueued = true;
            isGrounded = false;
            animator.SetTrigger("doJump");
            animator.SetBool("isJumping", true);
        }
    }
    public void OnWalk(InputAction.CallbackContext context)
    {
        isWalking = context.performed;
    }
}
