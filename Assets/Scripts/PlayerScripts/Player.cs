using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
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
    [SerializeField] [Range(0f, 1000f)] private float kickDamage;
    [SerializeField] [Range(0f, 1000f)] private float punchDamage;
    [SerializeField] [Range(0f, 1000f)] private float stompDamage;
    [SerializeField] [Range(0f, 100f)] private float attackRange;
    [SerializeField] [Range(0f, 100f)] private float stompRange;
    [SerializeField] [Range(0f, 100f)] private float danceRange;
    [SerializeField] [Range(1f, 5f)] private float playerSize = 1f;
    [SerializeField] [Range(1, 5)] private int maxEnergyPoints = 5;
    [SerializeField] [Range(1f, 10f)] private float energyPointRestoreTime = 2f;
    
    
    
    [SerializeField] LayerMask layerMask;

    [SerializeField] private Transform hitPoint;
    [SerializeField] private Transform stompPoint;
    [SerializeField] private Transform dancePoint;

    [SerializeField] private Transform pfVFXgigant;

    private ParticleSystem pfVFXwalk;
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
    private bool isGiant = false;
    private bool isDancingAura = false;

    private int punchCount = 0;

    private float punchAnimCoolDownTimer;
    private float punchCoolDownTimer;
    private float kickCoolDownTimer;
    
    private int currentEnergyPoints;
    private Coroutine energyRestoreCoroutine;
    private UltimateTimers timers;
    
    private void Awake()
    {
        if (Current != null)
            Debug.LogError("?????? ???????????????????? ???????? ?????????????????? ????????????!");
        Current = this;

        if (maxEnergyPoints == 0)
            maxEnergyPoints = 5;

        currentEnergyPoints = maxEnergyPoints;
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
        playerInput.PlayerController.Ultimate1.started += OnUltimate1;
        playerInput.PlayerController.Ultimate2.started += OnUltimate2;

        pfVFXwalk = GetComponentInChildren<ParticleSystem>();

        energyRestoreCoroutine = StartCoroutine(RestoreEnergyPoint());
        timers = GetComponent<UltimateTimers>();
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
        HandleDancingAura();
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
        
        if (input.y == 0 && input.x == 0)
            return;
        
        var rotationVector = new Vector3(input.y, 0, -input.x);
        rotationVector.Normalize();
        if (rotationVector != Vector3.zero)
        {
            transform.forward = rotationVector;
        }
        
        if (isGiant) 
            HandleStomping();
    }

    private void HandleJump(float movementDirectionY)
    {
        if (isJumpPressed && characterController.isGrounded && currentEnergyPoints > 0)
        {  
            currentMovement.y = jumpForce;
            animator.SetTrigger("JumpPressed");
            SoundHandleScript.Current.PlaySound(SoundEnum.JUMP_START,audioSource);
            currentEnergyPoints--;
            UIService.Current.ChangeSpheresAmount(currentEnergyPoints);
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
                        punchable.Punch(punchDamage);
                    }
                } else
                {
                    SoundHandleScript.Current.PlaySound(SoundEnum.WEAPON_SLASH, audioSource);
                }
                if (enemy.CompareTag("Heal"))
                {
                    IPunchable punchable = enemy.GetComponent<IPunchable>();
                    if (punchable != null)
                    {
                        punchable.Punch(punchDamage);
                    }
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

            punchCoolDownTimer = 0;
        }
    }
    private void HandleKick()
    {
        if (kickCoolDownTimer < kickCoolDown)
        {
            kickCoolDownTimer+=Time.deltaTime;
        }

        if (!isKicking || !(kickCoolDownTimer >= kickCoolDown)) 
            return;
        
        animator.SetTrigger("IsKicking");
        var hitEnemies = Physics.OverlapSphere(hitPoint.position, attackRange, layerMask);

        foreach (var enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                var kickable = enemy.GetComponent<IKickable>();
                if (kickable != null)
                {
                    SoundHandleScript.Current.PlaySound(SoundEnum.KICK_REACTION_ENEMY, audioSource);
                    kickable.Kick(kickDamage, kickForce, hitPoint.position);
                }
            }
            if (enemy.CompareTag("Interact"))
            {
                var kickable = enemy.GetComponent<IKickable>();
                if (kickable != null)
                {
                    kickable.Kick(0, kickForce, transform.position);
                    SoundHandleScript.Current.PlaySound(SoundEnum.KICK, audioSource);
                    kickable.Kick(0, kickForce, hitPoint.position);
                }
            }
            if (enemy.CompareTag("Heal"))
            {
                var kickable = enemy.GetComponent<IKickable>();
                if (kickable != null)
                {
                    kickable.Kick(kickDamage, kickForce, hitPoint.position);
                }
            }
        }

        kickCoolDownTimer = 0;
    }

    private void HandleStomping()
    {
        animator.SetTrigger("isGiant");
        var hitEnemies = Physics.OverlapSphere(stompPoint.position, stompRange, layerMask);

        foreach (var enemy in hitEnemies)
        {
            if (!enemy.CompareTag("Enemy")) 
                continue;
            
            var stompable = enemy.GetComponent<IStompable>();

            stompable?.Stomp(stompDamage);
        }
    }

    private void HandleDancingAura()
    {
        if (!isDancingAura)
            return;
        
        Collider[] hitEnemies = Physics.OverlapSphere(dancePoint.position, danceRange, layerMask);
        Debug.Log("DANCE AROUND");
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Debug.Log("DANCE FUCKER!");
                IDancable danceable = enemy.GetComponent<IDancable>();
                if (danceable != null)
                {
                    danceable.Dance();
                }
            }
        }
    }
    
    public void UltimateEffect(byte num, bool turner) //turner - ??????/????????
    {
        var cooldownData = new CooldownObservingDTO();
        
        switch (num)
        {
            case 0: // ???????????? ?????? ?????????????????? ??????????????
                if (turner)
                {
                    Instantiate(pfVFXgigant, transform.position, pfVFXgigant.rotation);
                    transform.localScale = new Vector3(playerSize, playerSize, playerSize);
                    cooldownData.InitialCooldownValue = timers.GetCooldown(num);
                    ObserverWithData<CooldownObservingDTO>.FireEvent(Events.LargeUltimateActivated, cooldownData);
                }
                else
                {
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }

                isGiant = turner;
                
                break;
            case 1: // ???????????? ?????? ?????????????????? ??????????
                isDancingAura = turner;
                cooldownData.InitialCooldownValue = timers.GetCooldown(num);
                ObserverWithData<CooldownObservingDTO>.FireEvent(Events.DanceUltimateActivated, cooldownData);
                break;
        }
        timers.SetUltimateTimer(num);
    }
    
    private void OnJump(InputAction.CallbackContext obj)
    {
        isJumpPressed = obj.ReadValueAsButton();
    }

    private void OnKick(InputAction.CallbackContext obj)
    {
        isKicking = obj.ReadValueAsButton();
        SoundHandleScript.Current.PlaySound(SoundEnum.WEAPON_SLASH, audioSource);
    }

    private void OnPunch(InputAction.CallbackContext obj)
    {
        isPunching = obj.ReadValueAsButton();
        SoundHandleScript.Current.PlaySound(SoundEnum.WEAPON_SLASH, audioSource);
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
        pfVFXwalk.Play();
        SoundHandleScript.Current.PlaySound(SoundEnum.PLAYER_STEP, audioSource);
    }
    
    private void OnJumpEnd()
    {
        Debug.Log("SOSALITY@!!!");
    }

    //=================
    private void OnUltimate1(InputAction.CallbackContext obj)
    {
        //if(!isGiant && this.GetComponent<UltimateTimers>().GetCooldown(0) <= 0) 
        if(!isGiant)
            UltimateEffect(0, true);
    }
    
    private void OnUltimate2(InputAction.CallbackContext obj)
    {
        //if (!isDancingAura && this.GetComponent<UltimateTimers>().GetCooldown(1)<=0)
        if(!isDancingAura)    
            UltimateEffect(1, true);
    }


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
        energyRestoreCoroutine = null;
    }

    private IEnumerator RestoreEnergyPoint()
    {
        while (true)
        {
            yield return new WaitForSeconds(energyPointRestoreTime);
            if (currentEnergyPoints >= maxEnergyPoints)
                currentEnergyPoints = maxEnergyPoints;
            else
                currentEnergyPoints++;
            
            UIService.Current.ChangeSpheresAmount(currentEnergyPoints);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(stompPoint.position, stompRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(dancePoint.position, danceRange);

        /*Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, reachTargetDistance);*/
    }
}
