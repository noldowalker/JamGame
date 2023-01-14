using System;
using System.Collections;
using System.Collections.Generic;
using AI.Enum;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAIControllerScript : MonoBehaviour
{
    public bool AIDisabled = false;
    [SerializeField] public EnemyType enemyType;
    [SerializeField] [Range(0.1f, 10f)] protected float reachTargetDistance;
    [SerializeField] [Range(1f, 1000f)] protected float damage;
    [SerializeField] [Range(0.1f, 10f)] protected float attackRate;
    [SerializeField] [Range(0f, 500f)] protected float speedMove;
    [SerializeField] protected Transform hitArea;
    [SerializeField] protected Collider WeaponHitArea;

    protected float damageRadius;
    protected float attackTime;

    protected AudioSource audioSource;
    protected NavMeshAgent navMesh;
    protected GameObject player;
    protected Collider playersCollider;
    protected Animator animator;

    private float gruntCooldownTimer = 0;
    private float gruntCooldown = 10;

    protected bool isPlayerHittedByCurrentAttack;
    protected EnemyState state;
    protected Coroutine waitCoroutine;
    
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        navMesh.stoppingDistance = reachTargetDistance;
        navMesh.speed = speedMove;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        if (player != null)
            playersCollider = player.GetComponent<Collider>();
        
        animator = GetComponent<Animator>();
        damageRadius = reachTargetDistance / 2;
        state = EnemyState.Idle;
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
        };
        PerformGrunt(10, 25); //Random voice sounds by enemies
    }
    
    public void ReactOnPunch()
    {
        if (enemyType == EnemyType.KING)
        {
            return;
        }
        if (state == EnemyState.Dying)
            return;
        
        state = EnemyState.Punched;
        animator.SetTrigger("IsPunched");
        ChangeStateAfterTime(1.2f, EnemyState.Idle);
    }

    public void ReactOnKick()
    {
        if(enemyType == EnemyType.KING)
        {
            return;
        }
        if (state == EnemyState.Dying)
            return;  
        state = EnemyState.Kicked;
        animator.SetTrigger("IsKicked");
        ChangeStateAfterTime(4f, EnemyState.Idle);
    }
    
    public void ReactOnDeath()
    {
        if (state == EnemyState.Dying)
            return;
        
        state = EnemyState.Dying;
        animator.SetTrigger("isDie");
        ChangeStateAfterTime(4f, EnemyState.SelfDestroy);
    }

    protected void TryFindPlayerAndReach()
    {
        if (player == null)
            return;
        
        FollowPoint(player.transform);
        state = EnemyState.MoveTowardsPlayer;
    }

    protected void MoveTowardsPlayer()
    {
        if (player == null)
            state = EnemyState.Idle;
        
        animator.SetBool("isRunning", true);
        FollowPoint(player.transform);

        if (!IsTargetAttackable(player.transform)) 
            return;
        
        state = EnemyState.Attacking;
        isPlayerHittedByCurrentAttack = false;
        animator.Play("Base Layer.Melee Attack");
        ChangeStateAfterTime(1.8f, EnemyState.Idle);
    }

    protected void FollowPoint(Transform point)
    {
        navMesh.destination = point.position;
        navMesh.speed = speedMove;
    }

    protected bool IsTargetAttackable(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) <= reachTargetDistance;
    }

    protected void CheckIfPlayeTouched()
    {
        if (isPlayerHittedByCurrentAttack)
            return;
        SoundHandleScript.Current.PlaySound(SoundEnum.WEAPON_SLASH, audioSource);

        if (!WeaponHitArea.bounds.Intersects(playersCollider.bounds)) 
            return;

        Debug.Log("INTERSECTS!");
        isPlayerHittedByCurrentAttack = true;
        IDamagable damagable = player.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.Damage(damage);
        }

        animator.Play("Base Layer.Melee Attack");
    }

    protected void Destroy()
    {
        Destroy(gameObject);
    }

    protected void PerformGrunt(float minInterval, float maxInterval)
    {
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

        waitCoroutine = StartCoroutine(WaitAndChangeState(waitTime, newState));
    }

    protected IEnumerator WaitAndChangeState(float waitTime, EnemyState newState)
    {
        yield return new WaitForSeconds(waitTime);
        state = newState;
        waitCoroutine = null;
    }

    protected void OnDestroy()
    {
        if (waitCoroutine != null)
            StopCoroutine(waitCoroutine);
    }
}
