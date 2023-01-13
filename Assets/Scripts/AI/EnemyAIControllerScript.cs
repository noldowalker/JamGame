using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIControllerScript : MonoBehaviour
{
    public bool AIDisabled = false;
    [SerializeField] public EnemyType enemyType;
    [SerializeField] [Range(0.1f, 10f)] private float reachTargetDistance;
    [SerializeField] [Range(1f, 1000f)] private float damage;
    [SerializeField] [Range(0.1f, 10f)] private float attackRate;
    [SerializeField] [Range(0f, 500f)] private float speedMove;

    private float damageRadius;
    private  float attackTime;

    [SerializeField] private Transform hitArea;

    private AudioSource audioSource;
    private NavMeshAgent navMesh;
    private GameObject player;
    private Animator animator;

    private bool isAttacking;

    private float gruntCooldownTimer = 0;
    private float gruntCooldown = 10;

    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
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
            if (FollowPlayer())
            {
                navMesh.speed = 0.0f;
                animator.SetBool("isRunning", false);
                if(isAttacking)
                HandlePunch();
            }
            PerformGrunt(10, 25); //Random voice sounds by enemies
            // animator.Play("Base Layer.RobotHipHopDance"); 
        }
    }

    bool FollowPlayer()
    {
        if(player !=null)
        return FollowAgent(player);

        return false;
    }

    public bool FollowPoint(Transform point)
    {
        navMesh.destination = point.position;
        animator.SetBool("isRunning", true);
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
        SoundHandleScript.Current.PlaySound(SoundEnum.WEAPON_SLASH, audioSource);

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

    private void PerformGrunt(float minInterval, float maxInterval)
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

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(hitArea.position, damageRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, reachTargetDistance);
    }


}
