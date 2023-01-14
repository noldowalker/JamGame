using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnSystem : MonoBehaviour
{
    public static SpawnSystem Current;
    [SerializeField] public int totalEnemiesOnScene;
    [SerializeField] private string nextScene;
    public int currentDeadEnemies;

    private void Awake()
    {
        Current = this;
    }

    void Start()
    {
        ObserverWithoutData.Sub(Events.EnemyDead, OnEnemyDead);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnemyDead()
    {
        currentDeadEnemies++;
        if(currentDeadEnemies >= totalEnemiesOnScene)
        {
            ToggleNextLevel();
        }
    }

    private void ToggleNextLevel()
    {
        SceneManager.LoadScene(nextScene);
    }

    private void OnDestroy()
    {
        ObserverWithoutData.Unsub(Events.EnemyDead, OnEnemyDead);
    }
}
