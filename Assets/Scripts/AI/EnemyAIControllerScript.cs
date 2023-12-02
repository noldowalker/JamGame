using System;
using System.Collections;
using System.Collections.Generic;
using AI.Enum;
using GameLogic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAIControllerScript : MonoBehaviour, IDancable
{
    public bool AIDisabled = false;
    public EnemyType EnemyType => enemyType;
    
    [SerializeField] [Range(0.1f, 10f)] protected float reachTargetDistance;
    [SerializeField] [Range(1f, 1000f)] protected float damage;
    [SerializeField] [Range(0f, 500f)] protected float speedMove;
    [SerializeField] protected Transform hitArea;
    [SerializeField] protected Collider WeaponHitArea;
    [SerializeField] protected EnemyType enemyType;

    protected float damageRadius;
    protected  float attackTime;

    protected AudioSource audioSource;
    protected NavMeshAgent navMesh;
    protected GameObject player;
    protected Collider playersCollider;
    protected Animator animator;
    protected Collider[] enemyColliders;
    protected Collider mainCollider;
    protected Rigidbody mainRigidbody;
    private Rigidbody[] rigidbodies;
    private Vector3 kickDirection;
    private bool isKicked;

    private float gruntCooldownTimer = 0;
    private float gruntCooldown = 10;

    protected bool isPlayerHittedByCurrentAttack;
    protected EnemyState state;
    protected Coroutine waitCoroutine;
    
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        mainCollider = GetComponent<Collider>();
        mainRigidbody = GetComponent<Rigidbody>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        enemyColliders = GetComponentsInChildren<Collider>();
        navMesh.stoppingDistance = reachTargetDistance;
        navMesh.speed = speedMove;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        if (player != null)
            playersCollider = player.GetComponent<Collider>();
        
        animator = GetComponent<Animator>();
        damageRadius = reachTargetDistance / 2;
        ChangeState(EnemyState.Idle);
        enableRagdoll(false);
    }

    void Update()
    {
        if (!AIDisabled)
            Act();
    }

    private void Act()
    {
        switch (state)
        {
            case EnemyState.Idle: 
                TryFindPlayerAndReach();
                break;
            case 
                EnemyState.MoveTowardsPlayer: 
                MoveTowardsPlayer();
                break;
            case EnemyState.Attacking:
                CheckIfPlayeTouched();
                break;
            case EnemyState.SelfDestroy:
                Destroy();
                break;
            case EnemyState.Dying:
                return;
        };
        
        PerformGrunt(10, 25); //Random voice sounds by enemies
    }

    public void enableRagdoll(bool enable)
    {
        
        foreach(Collider col in enemyColliders)
        {
            col.enabled = enable;
        }
        foreach(Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !enable;
        }
        animator.enabled = !enable;
        mainRigidbody.isKinematic = enable;
        mainCollider.enabled = !enable;
    }
    
    public void ReactOnPunch()
    {
        if (enemyType == EnemyType.KING)
        {
            return;
        }
        if (state == EnemyState.Dying)
        {
            return;
        }
        ChangeState(EnemyState.Punched);
        animator.SetTrigger("IsPunched");
        ChangeStateAfterTime(1.2f, EnemyState.Idle);
    }

    public void ReactOnKick(float force, Vector3 direction)
    {
        if(enemyType == EnemyType.KING)
        {
            return;
        }
        if (state == EnemyState.Dying)
        {
            return;
        }
        mainRigidbody.AddForce((transform.position - direction) * force, ForceMode.Impulse);
        ChangeState(EnemyState.Punched);
        animator.SetTrigger("IsPunched");
        ChangeStateAfterTime(1.2f, EnemyState.Idle);
    }

    public void ReactOnUppercut(float force)
    {
        if (enemyType == EnemyType.KING)
        {
            return;
        }
        if (state == EnemyState.Dying)
        {
            return;
        }
        mainRigidbody.AddForce((transform.up) * force, ForceMode.Impulse);
        ChangeState(EnemyState.Punched);
        animator.SetTrigger("IsPunched");
        ChangeStateAfterTime(3, EnemyState.Idle);
    }
    
    public void ReactOnDeath()
    {   
        ChangeState(EnemyState.Dying);
        animator.SetTrigger("IsDie");
        /*enemyCollider.enabled = false;
        WeaponHitArea.enabled = false;*/
        ChangeStateAfterTime(4f, EnemyState.SelfDestroy);
        enableRagdoll(true);
    }
    
    public void Dance()
    {
        if (state == EnemyState.Dying)
        {
            return;
        }
        if (state == EnemyState.Dancing)
            return;
        if (EnemyType == EnemyType.KING)
            return;

        animator.SetTrigger("IsDancing");
        ChangeState(EnemyState.Dancing);
        Debug.Log("IMMA FUCKER AND IM DANCING!");

        ChangeStateAfterTime(4f, EnemyState.Idle);
    }

    protected void TryFindPlayerAndReach()
    {
        if (player == null)
            return;
        
        FollowPoint(player.transform);
        ChangeState(EnemyState.MoveTowardsPlayer);
    }

    protected void MoveTowardsPlayer()
    {
        if (state == EnemyState.Dying)
        {
            return;
        }
        if (player == null)
            ChangeState(EnemyState.Idle);

        navMesh.enabled = true;
        animator.SetFloat("Speed", 1);
        FollowPoint(player.transform);

        if (!IsTargetAttackable(player.transform))
        {
            return;
        }
        
        ChangeState(EnemyState.Attacking);
        isPlayerHittedByCurrentAttack = false;
        animator.SetFloat("Speed", 0);
        animator.SetTrigger("Attack");
        ChangeStateAfterTime(10f, EnemyState.Idle);
    }

    protected void FollowPoint(Transform point)
    {
        if (state == EnemyState.Dying)
        {
            return;
        }
        if (!navMesh.enabled)
        {
            return;
        }
        navMesh.destination = point.position;
        navMesh.speed = speedMove;
    }

    protected bool IsTargetAttackable(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) <= reachTargetDistance;
    }

    protected void CheckIfPlayeTouched()
    {
        if (state == EnemyState.Dying)
        {
            return;
        }
        
        if (isPlayerHittedByCurrentAttack)
            return;
        SoundHandleScript.Current.PlaySound(SoundEnum.WEAPON_SLASH, audioSource);

        animator.Play("Base Layer.Melee Attack");
        if (!WeaponHitArea.bounds.Intersects(playersCollider.bounds)) 
            return;

        isPlayerHittedByCurrentAttack = true;
        IDamagable damagable = player.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.Damage(damage);
        }
    }

    protected void Destroy()
    {
        ObserverWithoutData.FireEvent(Events.EnemyDead);
        Destroy(gameObject);
    }

    protected void PerformGrunt(float minInterval, float maxInterval)
    {
        if (state == EnemyState.Dying)
        {
            return;
        }
        if (gruntCooldownTimer < gruntCooldown)
        {
            gruntCooldownTimer += Time.deltaTime;
        } else
        {
            switch (Random.Range(0, 2))
            {
                case 0: SoundHandleScript.Current.PlaySound(SoundEnum.ENEMY_GRUNT_1, audioSource);
                    break;
                case 1:
                    SoundHandleScript.Current.PlaySound(SoundEnum.ENEMY_GRUNT_2, audioSource);
                    break;
                case 2:
                    SoundHandleScript.Current.PlaySound(SoundEnum.ENEMY_GRUNT_3, audioSource);
                    break;
            }
            gruntCooldown = Random.Range(minInterval, maxInterval);
            gruntCooldownTimer = 0;
        }
    }

    protected void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(hitArea.position, damageRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, reachTargetDistance);
    }

    protected void ChangeStateAfterTime(float waitTime, EnemyState newState)
    {
        if (waitCoroutine != null)
            StopCoroutine(waitCoroutine);

        animator.SetBool("IsDancingBool", state == EnemyState.Dancing);
        
        
        waitCoroutine = StartCoroutine(WaitAndChangeState(waitTime, newState));
    }


    private void ChangeState(EnemyState newState)
    {
        state = newState;
        if (newState != EnemyState.MoveTowardsPlayer)
            navMesh.enabled = false;
        if (newState == EnemyState.Idle)
            animator.SetTrigger("ToIdle");
        animator.SetBool("IsDancingBool", state == EnemyState.Dancing);
        
        waitCoroutine = null;
    }
    
    private IEnumerator WaitAndChangeState(float waitTime, EnemyState newState)
    {
        yield return new WaitForSeconds(waitTime);
        ChangeState(newState);
        waitCoroutine = null;
    }

    protected void OnDestroy()
    {
        if (waitCoroutine != null)
            StopCoroutine(waitCoroutine);
    }
}