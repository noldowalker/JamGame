using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnScript : MonoBehaviour
{
    public List<GameObject> actorPrefabs;
    public bool canSpawnEnemies = true; //for debug
    public float respawnTimer;
    public int maxEnemiesToSpawn = 10;

    private float currentTimer;

    void Start()
    {
        currentTimer = 0;   
    }

    void Update()
    {
        if(canSpawnEnemies && CountAllEnemies() < maxEnemiesToSpawn)
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

    public int CountAllEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}
