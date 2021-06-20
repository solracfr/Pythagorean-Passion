using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private BoxCollider2D _boxCollider2D;
    [SerializeField] private float inputX;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float jumpFallSpeed;
    [SerializeField] private float jumpHoldTimer;
    [SerializeField] private int jumpsLeft;
    [SerializeField] private bool jumpPressed;
    [SerializeField] private bool playerIsJumping;
    [SerializeField] private bool playerIsGrounded;
    [SerializeField] private float stopThreshold;
    [SerializeField] private LayerMask platformLayerMask;

    private bool checkIfGrounded()
    {
        float extraHeightTolerance = 0.1f;
        RaycastHit2D hit = Physics2D.BoxCast(_boxCollider2D.bounds.center, _boxCollider2D.bounds.size, 0f, Vector2.down, extraHeightTolerance, platformLayerMask);

        Color rayColor;
        if (hit.collider != null) rayColor = Color.green;
        else rayColor = Color.red;

        Debug.DrawRay(_boxCollider2D.bounds.center + new Vector3(_boxCollider2D.bounds.extents.x, 0), Vector3.down * (_boxCollider2D.bounds.extents.y + extraHeightTolerance), rayColor);
        Debug.DrawRay(_boxCollider2D.bounds.center - new Vector3(_boxCollider2D.bounds.extents.x, 0), Vector3.down * (_boxCollider2D.bounds.extents.y + extraHeightTolerance), rayColor);
        Debug.DrawRay(_boxCollider2D.bounds.center - new Vector3(_boxCollider2D.bounds.extents.x, _boxCollider2D.bounds.extents.y + extraHeightTolerance), Vector3.right * (_boxCollider2D.bounds.extents.x), rayColor);
        //Debug.Log(hit.collider);

        playerIsGrounded = hit.collider != null;
        playerIsJumping = !playerIsGrounded;
        return playerIsGrounded;
    }

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        checkIfGrounded();
    }


    void FixedUpdate()
    {
        ProcessRun();
        ProcessJump();
    }

    private void ProcessJump()
    {
        const int maxJumpsLeft = 3;
        if (playerIsGrounded) jumpsLeft = maxJumpsLeft;

        if (jumpPressed && playerIsGrounded) 
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, jumpSpeed);
            jumpHoldTimer = 0.15f;
        }

        
        if (jumpPressed && playerIsJumping)
        {
            if (jumpHoldTimer > 0 && jumpsLeft > 0)
            {   
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, jumpSpeed);
                jumpHoldTimer -= Time.deltaTime;
            }
        } 
        

        if (!playerIsGrounded && _rigidBody.velocity.y <= 0f) _rigidBody.AddForce(new Vector2(0, -jumpFallSpeed), ForceMode2D.Impulse);
    }

    private void ProcessRun()
    {

        if (inputX == 0f && (_rigidBody.velocity.magnitude < stopThreshold)) _rigidBody.velocity = new Vector2(0f, _rigidBody.velocity.y); //keeps decceleration from feeling too gradual
        else _rigidBody.AddForce(new Vector2(inputX * movementSpeed * _rigidBody.drag * Time.fixedDeltaTime, 0f), ForceMode2D.Impulse); //* by drag eliminates need to fiddle with it in editor 
    }

    public void OnMove(InputAction.CallbackContext context) => inputX = context.ReadValue<float>();

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log(context.phase.ToString());
        if (context.performed) jumpsLeft--;
        context.action.performed += ctx => jumpPressed = true;
        context.action.canceled += ctx => jumpPressed = false;
        if (context.canceled) jumpHoldTimer = 0.05f;
    }
}
