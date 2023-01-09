using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIControllerScript : MonoBehaviour
{
    public bool AIDisabled = false;
    //����������� ���������� �� ����, �� ������� ����� ����������� �����
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
        if (!AIDisabled)
        {
            //�����
            if(animator != null)
            animator.Play("Base Layer.RobotHipHopDance");
        }
    }

    bool FollowPlayer()
    {
        return FollowAgent(player);
    }

    public bool FollowPoint(Transform point)
    {
        navMesh.destination = point.position;
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
