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
        currentTimer = respawnTimer;   
    }

    void Update()
    {
        int enemyCount = CountAllEnemies();
        if (canSpawnEnemies && enemyCount < maxEnemiesToSpawn && (SpawnSystem.Current.currentDeadEnemies + enemyCount) < SpawnSystem.Current.totalEnemiesOnScene)
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
