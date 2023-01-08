using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIControllerScript : MonoBehaviour
{
    //����������� ���������� �� ����, �� ������� ����� ����������� �����
    public float reachTargetDistance;

    private NavMeshAgent navMesh;
    private GameObject player;

    //����, ��������������, ����� �� ������� �� ���������� ��� �����
    private bool targetReached;
    
    void Start()
    {
        targetReached = false;
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.stoppingDistance = reachTargetDistance;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    void Update()
    {
        if (!targetReached)
        {
            targetReached = FollowPlayer();
        } else
        {
            //todo �����
        }
    }

    public bool FollowPlayer()
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
