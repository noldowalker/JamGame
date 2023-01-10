using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


public class Player : MonoBehaviour
{
    private PlayerInput playerInput;
    private CharacterController characterController;
   private Animator animator;

    [SerializeField] [Range(1f, 100f)] private float runSpeed;
    [SerializeField] [Range(1f, 100f)] private float walkSpeed;
    [SerializeField] [Range(1f, 100f)] private float jumpForce;
    [SerializeField] [Range(1f, 100f)] private float fallControlSpeed;
    [SerializeField] [Range(1f, 100f)] private float gravity;
    [SerializeField] [Range(0.1f, 10f)] private float punchCoolDown;
    [SerializeField] [Range(0.1f, 10f)] private float kickCoolDown;
    [SerializeField] [Range(1000f, 10000f)] private float kickForce;
    public static UnityEvent punchEvent = new UnityEvent();
    public static UnityEvent<float> kickEvent = new UnityEvent<float>();

    private Vector2 input;
    private Vector3 currentMovement;
    private bool isRunPressed;
    private bool isJumpPressed = false;
    private bool isFalling = false;
    private bool isMovementPressed;
    private bool isPunching;
    private bool isKicking;

    private float punchCoolDownTimer;
    private float kickCoolDownTimer;
    



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
        playerInput.PlayerController.Punch.started += OnPunch;
        playerInput.PlayerController.Punch.canceled += OnPunch;
        playerInput.PlayerController.Kick.started += OnKick;
        playerInput.PlayerController.Kick.canceled += OnKick;

    }

    private void Start()
    {
        punchCoolDownTimer = punchCoolDown;
        kickCoolDownTimer = kickCoolDown;
    }

    private void Update()
    {
        HandleMovement();
        HandlePunch();
        HandleKick();
    }

  

    private void HandleMovement()
    {
        animator.SetFloat("speed", 0);

        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;
        float speedMultipler = 1;
        if (isFalling)
        {
            speedMultipler = fallControlSpeed;
        }
        else
        {
            if (isMovementPressed)
            {
                if (isRunPressed)
                {
                    speedMultipler = runSpeed;
                    animator.SetFloat("speed", 2);
                }
                else
                {
                    speedMultipler = walkSpeed;
                    animator.SetFloat("speed", 1);
                }
            }
           
        }
        float curSpeedX = speedMultipler * -input.x;
        float curSpeedY = speedMultipler * input.y;
        float movementDirectionY = currentMovement.y;
        currentMovement = (forward * curSpeedX) + (right * curSpeedY);
        HandleJump(movementDirectionY);
        characterController.Move(currentMovement * Time.deltaTime);
        Quaternion currentRotation = transform.rotation;
        if (Vector3.Magnitude(currentMovement) > 2) 
        {
            transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(currentMovement).eulerAngles.y, 0);
        } else
        {
            transform.rotation = currentRotation;
        }
    }

    private void HandleJump(float movementDirectionY)
    {
        Debug.Log(characterController.isGrounded);

        if (isJumpPressed && characterController.isGrounded)
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

    private void HandlePunch()
    {
        if (punchCoolDownTimer < punchCoolDown)
        {
            punchCoolDownTimer+=Time.deltaTime;
        }
        if (isPunching && punchCoolDownTimer >= punchCoolDown)
        {
            print("Punch");
            punchEvent?.Invoke();
            punchCoolDownTimer = 0;
        }
    }

    private void HandleKick()
    {
        if (kickCoolDownTimer < kickCoolDown)
        {
            kickCoolDownTimer+=Time.deltaTime;
        }
        if (isKicking && kickCoolDownTimer >= kickCoolDown)
        {
            print("Kick");
            kickEvent?.Invoke(kickForce);
            kickCoolDownTimer = 0;
        }
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        isJumpPressed = obj.ReadValueAsButton();
    }

    private void OnKick(InputAction.CallbackContext obj)
    {
        isKicking = obj.ReadValueAsButton();
    }

    private void OnPunch(InputAction.CallbackContext obj)
    {
        isPunching = obj.ReadValueAsButton();
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
