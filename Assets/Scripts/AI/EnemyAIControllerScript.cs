using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIControllerScript : MonoBehaviour
{
    //минимальное расстояние до цели, на котором может проводиться атака
    public float reachTargetDistance;

    private NavMeshAgent navMesh;
    private GameObject player;
    private Animator animator;

    
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.stoppingDistance = reachTargetDistance;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (FollowPlayer())
        {
            animator.SetBool("isRunning", false);

            //атака  либо ставить кд на атаку и проигрывать при каждой атаке, либо в анимации поставить луп
            animator.Play("Base Layer.Melee Attack"); 

            // animator.Play("Base Layer.RobotHipHopDance");  перенсти в ульту
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
        if (Vector3.Distance(transform.position, point.transform.position) <= reachTargetDistance)
        {
            return true;
        }
        else return false;
    }

    public bool FollowAgent(GameObject agent)
    {
        return FollowPoint(agent.transform);
    }
}
