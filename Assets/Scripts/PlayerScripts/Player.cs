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
    [SerializeField] [Range(1000f, 10000f)] private float kickForce;
    [SerializeField] [Range(1000f, 10000f)] private float uppercutForce;
    [SerializeField] [Range(0f, 1000f)] private float kickDamage;
    [SerializeField] [Range(0f, 1000f)] private float punchDamage;
    [SerializeField] [Range(0f, 1000f)] private float stompDamage;
    [SerializeField] [Range(0f, 100f)] private float attackRange;
    [SerializeField] [Range(0f, 100f)] private float stompRange;
    [SerializeField] [Range(0f, 100f)] private float danceRange;
    [SerializeField] [Range(1f, 5f)] private float playerSize = 1f;
    [SerializeField] [Range(1, 5)] private int maxEnergyPoints = 5;
    [SerializeField] [Range(1f, 10f)] private float energyPointRestoreTime = 2f;
    [SerializeField] [Range(0f, 0.05f)] private float hitDashForce;
    
    
    
    [SerializeField] LayerMask layerMask;

    [SerializeField] private Transform hitPoint;
    [SerializeField] private Transform stompPoint;
    [SerializeField] private Transform dancePoint;

    [SerializeField] private Transform pfVFXgigant;

    private AudioSource audioSource;

    public static UnityEvent punchEvent = new UnityEvent();
    public static UnityEvent<float> kickEvent = new UnityEvent<float>();
    public static Player Current;
    public PlayerInput Input => playerInput;

    public ParticleSystem pfvfxWalk;
    private Vector2 input;
    private Vector3 currentMovement;

    private bool isRunPressed;
    private bool isJumpPressed = false;
    private bool isFalling = false;
    private bool isMovementPressed;
    private bool isPunching;
    private bool isKicking;
    private bool isUppercut;
    private bool isGiant = false;
    private bool isDancingAura = false;
    private bool isCanPunch = true;

    private int punchCount = 0;

    private float punchAnimCoolDownTimer;
    private float punchCoolDownTimer;
    private float kickCoolDownTimer;
    private float danceDamageCoolDownTimer;
    
    private int currentEnergyPoints;
    private Coroutine energyRestoreCoroutine;
    private UltimateTimers timers;
    private bool isCanMove;
    private GameObject magneticTarget;


    private void Awake()
    {
        if (Current != null)
            Debug.LogError("Уже существует один экземпляр игрока!");
        Current = this;
        isCanMove = true;
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
        playerInput.PlayerController.Uppercut.started += OnUppercut;

        energyRestoreCoroutine = StartCoroutine(RestoreEnergyPoint());
        timers = GetComponent<UltimateTimers>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        HandleMovement();
        HandleDancingAura();
        HandlePunch();
        HandleUppercut();
    }
    private void HandleMovement()
    {
        if (isCanMove)
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
    }

    private void HandleJump(float movementDirectionY)
    {
        if (isJumpPressed && characterController.isGrounded)
        {  
            currentMovement.y = jumpForce;
            animator.SetTrigger("JumpPressed");
            SoundHandleScript.Current.PlaySound(SoundEnum.JUMP_START,audioSource);
            //currentEnergyPoints--;
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

   
   /* private void CoolDown()
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
    }*/

    private GameObject determineTarget()
    {
        Collider[] targets = Physics.OverlapSphere(hitPoint.position, attackRange * 3, layerMask);
        foreach (Collider target in targets)
        {
            Debug.Log(target.tag);
            if (target.CompareTag("Enemy"))
            {
                return target.gameObject;
            }
        }
        return null;
    }

    private void HandleUppercut()
    {
        if (isCanPunch)
        {
            if (isUppercut)
            {
                isUppercut = false;
                animator.SetTrigger("isUppercut");
            }
        }
    }

    private void UppercutJump()
    {
        currentMovement.y = jumpForce;
    }

    private void HandlePunch()
    {
        if(isCanPunch)
        {
            if (!isCanMove)
            {
                isCanMove = true;
            }
            if (isPunching || isKicking)
            {
                SoundHandleScript.Current.PlaySound(SoundEnum.WEAPON_SLASH, audioSource);
                magneticTarget = determineTarget();

                if (isPunching)
                {
                    isCanPunch = false;
                    isPunching = false;
                    isCanMove = false;

                    punchCount++;
                    if (punchCount > 3)
                    {
                        punchCount = 1;
                    }
                    animator.SetTrigger("IsPunching");
                    animator.SetInteger("Punch", punchCount);
                }
                if (isKicking)
                {
                    isCanPunch = false;
                    isKicking = false;
                    isCanMove = false;
                    animator.SetTrigger("IsKicking");
                }
            }
        } else
        {
            animator.SetFloat("speed", 2);
            if (magneticTarget != null)
            {
                Vector3 target = new Vector3(magneticTarget.transform.position.x,
                    transform.position.y, magneticTarget.transform.position.z);
                transform.LookAt(target);
                if(Vector3.Distance(transform.position,magneticTarget.transform.position) > 1.5f)
                {
                    characterController.Move(transform.forward * hitDashForce);
                }
            }
            else
            {
                characterController.Move(transform.forward * hitDashForce);
            }
        }

    }

    public void HandleHitDamage()
    {
        isPunching = false;
        isCanPunch = true;
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
    }

    public void HandleUppercutDamage()
    {
        isPunching = false;
        isCanPunch = true;
        Collider[] hitEnemies = Physics.OverlapSphere(hitPoint.position, attackRange*5, layerMask);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                IUppercutable punchable = enemy.GetComponent<IUppercutable>();
                if (punchable != null)
                {
                    punchable.Uppercut(punchDamage, uppercutForce);
                }
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
    }

    public void HandleHitEnd()
    {
        punchCount = 0;
    }

    public void HandleKickDamage()
    {
        isCanPunch = true;
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
    }


    private void HandleStomping()
    {
        //animator.SetTrigger("isGiant");
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
        {
            playerInput.PlayerController.Enable();
            return;
        }
        playerInput.PlayerController.Disable();
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
                    if (danceDamageCoolDownTimer < 1.5f)
                    {
                        danceDamageCoolDownTimer += Time.deltaTime;
                    }
                    else
                    {
                        danceDamageCoolDownTimer = 0;
                        IPunchable punchable = enemy.GetComponent<IPunchable>();
                        punchable.Punch(punchDamage);
                    }
                }
               
            }
        }
    }
    
    public void UltimateEffect(byte num, bool turner) //turner - вкл/выкл
    {
        var cooldownData = new CooldownObservingDTO();
        
        switch (num)
        {
            case 0: // Запуск или остановка гиганта
                if (turner)
                {
                    Instantiate(pfVFXgigant, transform.position, pfVFXgigant.rotation);
                    transform.localScale = new Vector3(playerSize, playerSize, playerSize);
                    cooldownData.InitialCooldownValue = timers.GetCooldown(num) + timers.GetUltimateDuration(num);
                    ObserverWithData<CooldownObservingDTO>.FireEvent(Events.LargeUltimateActivated, cooldownData);
                    animator.SetTrigger("isGiant");
                    MusicHandleScript.Current.SwitchCurrentMusic(MusicEnum.MUSIC_KAZACHOCK);
                }
                else
                {
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }

                isGiant = turner;
                
                break;
            case 1: // Запуск или остановка танца
                isDancingAura = turner;
                animator.SetTrigger("isSinging");
                cooldownData.InitialCooldownValue = timers.GetCooldown(num) + timers.GetUltimateDuration(num);;
                ObserverWithData<CooldownObservingDTO>.FireEvent(Events.DanceUltimateActivated, cooldownData);
                MusicHandleScript.Current.SwitchCurrentMusic(MusicEnum.MUSIC_FISH);
                break;
        }
        timers.SetUltimateTimer(num);
    }


    private void OnUppercut(InputAction.CallbackContext obj)
    {
        if (isGiant || isDancingAura)
            return;
        isUppercut = obj.ReadValueAsButton();
    }
    
    private void OnJump(InputAction.CallbackContext obj)
    {
        if (isGiant || isDancingAura)
            return;
        isJumpPressed = obj.ReadValueAsButton();
    }

    private void OnKick(InputAction.CallbackContext obj)
    {
        if (isGiant || isDancingAura)
            return;
        isKicking = obj.ReadValueAsButton();
    }

    private void OnPunch(InputAction.CallbackContext obj)
    {
        if (isGiant || isDancingAura)
            return;
        isPunching = obj.ReadValueAsButton();
    }
    
    private void OnRun(InputAction.CallbackContext obj)
    {
        if (isGiant || isDancingAura)
            return;
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
        ParticleSystem currentPfWalk = Instantiate(pfvfxWalk, this.transform);
        currentPfWalk.Play();

        SoundHandleScript.Current.PlaySound(SoundEnum.PLAYER_STEP, audioSource);
    }
    
    private void OnJumpEnd()
    {
    }

    //=================
    private void OnUltimate1(InputAction.CallbackContext obj)
    {
        //if(!isGiant && this.GetComponent<UltimateTimers>().GetCooldown(0) <= 0) 
        if(!isGiant && !isDancingAura && !timers.isCooldownTimerOn[0])
            UltimateEffect(0, true);
    }
    
    private void OnUltimate2(InputAction.CallbackContext obj)
    {
        //if (!isDancingAura && this.GetComponent<UltimateTimers>().GetCooldown(1)<=0)
        if(!isDancingAura && !isGiant && !timers.isCooldownTimerOn[1])    
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
            
            //UIService.Current.ChangeSpheresAmount(currentEnergyPoints);
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
