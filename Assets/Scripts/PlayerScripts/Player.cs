using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    private PlayerInput playerInput;
    private CharacterController characterController;
   private Animator animator;

    [SerializeField] [Range(1f, 100f)] private float runSpeed;
    [SerializeField] [Range(1f, 100f)] private float walkSpeed;
    [SerializeField] [Range(1f, 100f)] private float jumpForce;
    [SerializeField] [Range(1f, 100f)] private float fallControlSpeed;
    [SerializeField] [Range(0f, 100f)] private float gravity;

    private Vector2 input;
    private Vector3 currentMovement;

    private bool isRunPressed;
    private bool isJumpPressed = false;
    private bool isFalling = false;
    private bool isMovementPressed;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        playerInput.PlayerController.Move.started += OnMovementInput;
        playerInput.PlayerController.Move.canceled += OnMovementInput;
        playerInput.PlayerController.Move.performed += OnMovementInput;
        playerInput.PlayerController.Run.started += OnRun;
        playerInput.PlayerController.Run.canceled += OnRun;
        playerInput.PlayerController.Jump.started += OnJump;
        playerInput.PlayerController.Jump.canceled += OnJump;


    }

    private void Start()
    {
        
    }

    private void Update()
    {
        HandleMovement();
    }

  

    private void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float speedMultipler;
        if (isFalling)
        {
            speedMultipler = fallControlSpeed;
        }
        else
        {
            if (isRunPressed)
            {
                speedMultipler = runSpeed;
                animator.SetBool("isRunning", true);
                animator.SetBool("isWalk", false);
            } else
            {
                speedMultipler = walkSpeed;
                animator.SetBool("isWalk", true);
                animator.SetBool("isRunning", false);
            }
        }
        if (!isMovementPressed)
        {
            animator.SetBool("isWalk", false);
            animator.SetBool("isRunning", false);
        }
        float curSpeedX = speedMultipler * -input.x;
        float curSpeedY = speedMultipler * input.y;
        float movementDirectionY = currentMovement.y;
        currentMovement = forward * curSpeedX + right * curSpeedY;
        HandleJump(movementDirectionY);
        characterController.Move(currentMovement * Time.deltaTime);
    }

    private void HandleJump(float movementDirectionY)
    {
        if (isJumpPressed && !isFalling)
        {
            currentMovement.y = jumpForce;
            animator.Play("Base Layer.Jump Attack");  
        } else
        {
            currentMovement.y = movementDirectionY;
        }
        if (!characterController.isGrounded)
        {
            if (currentMovement.y < 0f)
            {
                isFalling = true;
            }
            currentMovement.y -= gravity * Time.deltaTime;
        }
        else
        {
            isFalling = false;
        }
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        isJumpPressed = obj.ReadValueAsButton();
    }
    private void OnRun(InputAction.CallbackContext obj)
    {
        isRunPressed = obj.ReadValueAsButton();
    }

    private void OnMovementInput(InputAction.CallbackContext obj)
    {
        input = obj.ReadValue<Vector2>();
        isMovementPressed = input.x != 0 || input.y != 0;

    }


    private void OnEnable()
    {
        playerInput.PlayerController.Enable();
    }

    private void OnDisable()
    {
        playerInput.PlayerController.Disable();
    }
}
