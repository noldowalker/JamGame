using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KingController : MonoBehaviour
{
    public bool AIDisabled = false;

    [SerializeField] [Range(0.1f, 10f)] private float reachTargetDistance;

    [SerializeField] [Range(1f, 1000f)] private float damage;
    [SerializeField] [Range(0.1f, 10f)] private float attackRate;
    [SerializeField] [Range(0.1f, 10f)] private float rollRate;
    [SerializeField] [Range(0.1f, 10f)] private float timeOfRoll;
    [SerializeField] [Range(0f, 500f)] private float speedMove;
    [SerializeField] [Range(0f, 500f)] private float rollSpeed;

    private float damageRadius;

    private float attackTime;
    private float rollTime;

    [SerializeField] private Transform hitArea;
    [SerializeField] private Transform pfVFXposion;

    private NavMeshAgent navMesh;
    private GameObject player;
    private Animator animator;

    private bool isAttacking;
    private bool isRolling;
    private bool isReadyToRoll;

    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.stoppingDistance = reachTargetDistance;
        navMesh.speed = speedMove;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        animator = GetComponent<Animator>();
        damageRadius = reachTargetDistance / 2;
    }

    void Update()
    {
        if (!AIDisabled)
        {
            UpdateUltimate(Time.time);

            if (!isRolling && isReadyToRoll)
            {
                StartCoroutine(Rolling());
            }
            else
            {
                animator.SetBool("isRolling", false);

                if (FollowPlayer())
                {
                    navMesh.speed = 0.0f;
                    animator.SetBool("isWalking", false);
                    if (isAttacking)
                        HandlePunch();
                }
            }

            // animator.Play("Base Layer.RobotHipHopDance"); 
        }
    }

    IEnumerator Rolling()
    {
        isRolling = true;
        Instantiate(pfVFXposion, transform.position, transform.rotation);
        animator.SetBool("isRolling", true);
        navMesh.speed = rollSpeed;
        navMesh.destination = player.transform.position;

        yield return new WaitForSeconds(timeOfRoll); 
        isRolling = false;
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

    private void UpdateUltimate(float deltaTime)
    {
        float fireInterval = 1.0f / rollRate;

        if (deltaTime > rollTime)
        {
            isReadyToRoll = true;
            rollTime = deltaTime + fireInterval;
        }
        else
        {
            isReadyToRoll = false;
        }
    }
}
