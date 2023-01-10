using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIControllerScript : MonoBehaviour
{
    public bool AIDisabled = false;
    //минимальное расстояние до цели, на котором может проводиться атака
    public float reachTargetDistance;

    private NavMeshAgent navMesh;
    private GameObject player;

    
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.stoppingDistance = reachTargetDistance;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    void Update()
    {
        if (!AIDisabled)
        {
            if (FollowPlayer())
            {
                //атака
            }
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
