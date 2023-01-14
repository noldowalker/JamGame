using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KingController : EnemyAIControllerScript
{
    [SerializeField] [Range(1f, 1000f)] private float rollDamage;
    [SerializeField] [Range(1f, 1000f)] private float explosionDamage;
    [SerializeField] [Range(1f, 30f)] private float timeOfRoll;
    [SerializeField] [Range(1f, 30f)] private float timeOfExplosion;
    [SerializeField] [Range(1f, 60f)] private float ultimateCoolDown;
    [SerializeField] [Range(1f, 25f)] private float rollSpeed;
    [SerializeField] [Range(1f, 25f)] private float explosionRadius;
    [SerializeField] [Range(1f, 25f)] private float explosionChargeTime;
    [SerializeField] [Range(1f, 25f)] private float attackRate;
    [SerializeField] private bool toggleFriendlyFire;
    private float rollTimer;
    private float rollChargeTimer;
    private float explosionTimer;
    private float ultimateCoolDownTimer;
    private float defaultSphereRadius;
    private float explosionChargeTimer;

    [SerializeField] private Transform pfVFXposion;
    private SphereCollider sphereTrigger;

    private bool isAttacking;
    private bool isRolling = false;
    private bool isReadyToRoll = false;
    private bool isReadyToExplode = false;
    private bool isExploding = false;

    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.stoppingDistance = reachTargetDistance;
        navMesh.speed = speedMove;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        animator = GetComponent<Animator>();
        damageRadius = reachTargetDistance / 2;
        sphereTrigger = GetComponent<SphereCollider>();
        defaultSphereRadius = sphereTrigger.radius;
    }

    void Update()
    {
        if (!AIDisabled)
        {
            if(state == AI.Enum.EnemyState.Dying)
            {
                AIDisabled = true;
                print("KING IS DEAD. GLORY TO THE KING");
                Destroy();
            }
            UpdateUltimate();
            HandleRolling();
            HandleExplode();
            if (navMesh.enabled)
            {
                if (FollowPlayer())
                {
                    if (isAttacking)
                        HandlePunch();
                }
            }
        }
    }

    void HandleRolling()
    {
        if (isReadyToRoll)
        {
            navMesh.enabled = false;
            animator.SetBool("isRolling", true);
            transform.localScale += new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
            if (rollChargeTimer < 5)
            {
                rollChargeTimer += Time.deltaTime;
            } else
            {
                rollChargeTimer = 0;
                isRolling = true;
                isReadyToRoll = false;
                transform.localScale = Vector3.one;
            }
        }
        if (isRolling)
        {
            
            Rolling();
        }
        else
        {
            animator.SetBool("isRolling", false);
        }
    }

    void Rolling()
    {
        transform.position += transform.forward * rollSpeed * Time.deltaTime;
        if (rollTimer < timeOfRoll)
        {
            rollTimer += Time.deltaTime;
        } else
        {
            navMesh.enabled = true;
            rollTimer = 0;
            isRolling = false;
        }
        
    }

    bool FollowPlayer()
    {
        if (player != null)
        {
            return FollowAgent(player);
        }

        return false;
    }

    public bool FollowPoint(Transform point)
    {
        navMesh.destination = point.position;
        animator.SetBool("isWalking", true);
        navMesh.speed = speedMove;
        if (Vector3.Distance(transform.position, point.transform.position) <= reachTargetDistance)
        {
            UpdateFiring(Time.time);
            return true;
        }
        else
            return false;
    }

    public bool FollowAgent(GameObject agent)
    {
        return FollowPoint(agent.transform);
    }
    private void HandlePunch()
    {

        Collider[] hitEnemies = Physics.OverlapSphere(hitArea.position, damageRadius);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Player"))
            {
                IDamagable damagable = enemy.GetComponent<IDamagable>();
                if (damagable != null)
                {
                    damagable.Damage(damage);
                }
            }
        }

        animator.Play("Base Layer.Melee Attack");

    }

    private void HandleExplode()
    {
        if (isReadyToExplode)
        {
            if(explosionChargeTimer < explosionChargeTime)
            {
                ChargeExplosion();
            } else
            {
                explosionChargeTimer = 0;
                isReadyToExplode = false;
                transform.localScale = Vector3.one;
                Explode();
            }
        }
        if (isExploding)
        {
            if (explosionTimer < timeOfExplosion)
            {
                explosionTimer += Time.deltaTime;
            }
            else
            {
                navMesh.enabled = true;
                explosionTimer = 0;
                isExploding = false;
                sphereTrigger.radius = defaultSphereRadius;
            }
        }
    }

    private void ChargeExplosion()
    {
        navMesh.enabled = false;
        transform.localScale += new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
        explosionChargeTimer += Time.deltaTime;
    }

    private void Explode()
    {
        isExploding = true;
        sphereTrigger.radius = explosionRadius;
        var explosion = Instantiate(pfVFXposion, transform.position, transform.rotation);
        explosion.localScale *= explosionRadius/2;
    }

    private void UpdateFiring(float deltaTime)
    {
        float fireInterval = 1.0f / attackRate;

        if (deltaTime > attackTime)
        {
            isAttacking = true;
            attackTime = deltaTime + fireInterval;
        }
        else
        {
            isAttacking = false;
        }
    }

    private void UpdateUltimate()
    {
        if (!isRolling && !isExploding)
        {
            if (ultimateCoolDownTimer < ultimateCoolDown)
            {
                ultimateCoolDownTimer += Time.deltaTime;
            }
            else
            {
                ultimateCoolDownTimer = 0;
                switch (Random.Range(0, 2))
                {
                    case 0: isReadyToRoll = true;
                        break;
                    case 1: isReadyToExplode = true;
                        break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isRolling || isExploding)
        {
            if (other.gameObject.isStatic)
            {
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 135, 0);
            }
            if (other.CompareTag("Player"))
            {
                IDamagable damagable = other.GetComponent<IDamagable>();
                if (damagable != null)
                {
                    damagable.Damage(isRolling ? rollDamage : explosionDamage);
                }
            }
            if (other.CompareTag("Enemy") && toggleFriendlyFire)
            {
                IPunchable punchable = other.GetComponent<IPunchable>();
                if (punchable != null)
                {
                    punchable.Punch(isRolling ? rollDamage : explosionDamage);
                }
            }
        }
    }
}
