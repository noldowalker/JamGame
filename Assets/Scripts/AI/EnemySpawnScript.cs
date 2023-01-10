using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnScript : MonoBehaviour
{
    //������ �������� �������� ��� ������
    public List<GameObject> actorPrefabs;
    public float respawnTimer;
    public float maxEnemiesToSpawn;

    private float currentTimer;

    void Start()
    {
        currentTimer = 0;   
    }

    void Update()
    {
        SpawnByTimer();
    }

    public void SpawnByTimer()
    {
        currentTimer += Time.deltaTime;
        if (currentTimer >= respawnTimer)
        {
            SpawnRandomActor();
            currentTimer = 0;
        }
    }

    public void SpawnRandomActor()
    {
        SpawnActor(actorPrefabs[Random.Range(0, actorPrefabs.Count)]);
    }

    public void SpawnActor(GameObject actor)
    {
        Instantiate(actor, transform.position, Quaternion.identity);
    }
}
