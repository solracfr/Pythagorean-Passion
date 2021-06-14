using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private float inputX;
    [SerializeField] private float movementSpeed = 20f;
    [SerializeField] private float jumpInitialSpeed = 20f;
    [SerializeField] private bool isJumping;
    [SerializeField] private float stopThreshold = 1f;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        //todo check if grounded: https://www.youtube.com/watch?v=c3iEl5AwUF8
        ProcessRun();
        ProcessJump();
    }

    private void ProcessRun()
    {
        if (inputX == 0f && (_rigidBody.velocity.magnitude < stopThreshold)) _rigidBody.velocity = new Vector2(0f, _rigidBody.velocity.y); //keeps decceleration from feeling too gradual
        else _rigidBody.AddForce(new Vector2(inputX * movementSpeed * _rigidBody.drag * Time.fixedDeltaTime, 0f), ForceMode2D.Impulse); //* by drag eliminates need to fiddle with it in editor
    }

    private void ProcessJump()
    {
        //todo reimplement with isGrounded check
        if (isJumping) _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, jumpInitialSpeed); 
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputX = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJumping = context.ReadValueAsButton();
        Debug.Log("Jumping!");
    }
}
