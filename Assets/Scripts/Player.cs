using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private bool isWalking;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector3 moveDirection;

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
        moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        float currentSpeed = isWalking ? walkSpeed : runSpeed;
        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

        if (moveDirection != Vector3.zero)
        {
            transform.LookAt(transform.position + moveDirection);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
        animator.SetBool("isJumping", false);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isGrounded = false;
            animator.SetTrigger("doJump");
            animator.SetBool("isJumping", true);
        }
    }
    public void OnWalk(InputAction.CallbackContext context)
    {
        if (context.performed)
            isWalking = true;
        else if (context.canceled)
            isWalking = false;
    }
}
