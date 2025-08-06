using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class Player : MonoBehaviour
{
    [SerializeField] private float runSpeed;
    [SerializeField] private bool isWalking;
    [SerializeField] private Vector2 inputVec;
    [SerializeField] private Vector3 moveVec;
    private Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        moveVec = new Vector3(inputVec.x, 0f, inputVec.y) * runSpeed * Time.deltaTime;
        transform.position += moveVec;

        anim.SetBool("isRunning", moveVec != Vector3.zero);
        anim.SetBool("isWalking", isWalking);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        inputVec = context.ReadValue<Vector2>();
    }
    public void OnWalk(InputAction.CallbackContext context)
    {
        if (context.performed)
            isWalking = true;

        if (context.canceled)
            isWalking = false;
    }
}
