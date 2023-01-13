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
    [SerializeField] [Range(0f, 1000f)] private float damage;
    [SerializeField] [Range(0f, 100f)] private float attackRange;

    [SerializeField] LayerMask layerMask;

    [SerializeField] private Transform hitPoint;

    private AudioSource audioSource;

    public static UnityEvent punchEvent = new UnityEvent();
    public static UnityEvent<float> kickEvent = new UnityEvent<float>();
    public static Player Current;
    public PlayerInput Input => playerInput;
    
    private Vector2 input;
    private Vector3 currentMovement;

    private bool isRunPressed;
    private bool isJumpPressed = false;
    private bool isFalling = false;
    private bool isMovementPressed;
    private bool isPunching;
    private bool isKicking;

    private int punchCount = 0;

    private float punchAnimCoolDownTimer;
    private float punchCoolDownTimer;
    private float kickCoolDownTimer;
  
    private void Awake()
    {
        if (Current != null)
            Debug.LogError("Уже существует один экземпляр игрока!");
        Current = this;
        
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
        punchAnimCoolDownTimer = 0;
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
        if (currentMovement.x + currentMovement.z != 0) 
        {
            transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(currentMovement).eulerAngles.y, 0);
        } else
        {
            transform.rotation = currentRotation;
        }
    }

    private void HandleJump(float movementDirectionY)
    {

        if (isJumpPressed && characterController.isGrounded)
        {  
            currentMovement.y = jumpForce;
            animator.SetTrigger("JumpPressed");
            SoundHandleScript.Current.PlaySound(SoundEnum.JUMP_START,audioSource);
        } else
        {
            currentMovement.y = movementDirectionY;
        }
        if (!characterController.isGrounded)
        {
            if (currentMovement.y < 0f)
            {
                isFalling = true;
                animator.SetBool("IsFalling", true);
            }
            
            currentMovement.y -= gravity * Time.deltaTime;
        }
        else
        {
            isFalling = false;
            animator.SetBool("IsFalling", false);
        }
        
    }

   
    private void CoolDown()
    {
        if (punchCoolDownTimer < punchCoolDown)
        {
            punchCoolDownTimer += Time.deltaTime;
        }
        if (punchCount != 0)
        {
            if (punchAnimCoolDownTimer < punchCoolDown + 0.5)
            {
                punchAnimCoolDownTimer += Time.deltaTime;
            }
            else
            {
                punchCount = 0;
                animator.SetInteger("Punch", punchCount);
                punchAnimCoolDownTimer = 0;
            }
        }
    }

    private void HandlePunch()
    {
        CoolDown();
        if (isPunching && punchCoolDownTimer >= punchCoolDown)
        {

            Collider[] hitEnemies = Physics.OverlapSphere(hitPoint.position, attackRange, layerMask);

            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    IPunchable punchable = enemy.GetComponent<IPunchable>();
                    if (punchable != null)
                    {
                        punchable.Punch(damage);
                    }
                } else
                {
                    SoundHandleScript.Current.PlaySound(SoundEnum.WEAPON_SLASH, audioSource);
                }
            }

            animator.SetTrigger("IsPunching");
            punchAnimCoolDownTimer = 0;
            if (punchCount >= 3)
            {
                punchCount = 0;
            }
            else
            {
                punchCount++;
            }
            
            animator.SetInteger("Punch", punchCount);
           // punchEvent?.Invoke();  // delete line
            punchCoolDownTimer = 0;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(hitPoint.position, attackRange);
    }

    private void HandleKick()
    {
        if (kickCoolDownTimer < kickCoolDown)
        {
            kickCoolDownTimer+=Time.deltaTime;
        }
        if (isKicking && kickCoolDownTimer >= kickCoolDown)
        {
            animator.SetTrigger("IsKicking");
            Collider[] hitEnemies = Physics.OverlapSphere(hitPoint.position, attackRange, layerMask);

            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    IKickable kickable = enemy.GetComponent<IKickable>();
                    if (kickable != null)
                    {
                        kickable.Kick(damage, kickForce, hitPoint.position);
                    }
                }
                if (enemy.CompareTag("Interact"))
                {
                    IKickable kickable = enemy.GetComponent<IKickable>();
                    if (kickable != null)
                    {
                        kickable.Kick(0, kickForce, transform.position);
                        SoundHandleScript.Current.PlaySound(SoundEnum.KICK, audioSource);
                        kickable.Kick(0, kickForce, hitPoint.position);
                    }
                }
            }

            //kickEvent?.Invoke(kickForce);
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

    //SoundEvents======

    private void OnStep()
    {
        //print("Step");
        SoundHandleScript.Current.PlaySound(SoundEnum.PLAYER_STEP, audioSource);
    }
    private void OnGotHit()
    {
        SoundHandleScript.Current.PlaySound(SoundEnum.HIT_REACTION, audioSource);
    }

    //=================

    private void OnEnable()
    {
        playerInput.PlayerController.Enable();
    }

    private void OnDisable()
    {
        playerInput.PlayerController.Disable();
    }

    private void OnDestroy()
    {
        Current = null;
    }
}
