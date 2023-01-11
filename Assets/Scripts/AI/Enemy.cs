using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHP;
    private EnemyAIControllerScript controller;
    private Player player;
    private Animator animator;
    private HealthSystem healthSystem;
    private bool playerCanHit;
    void Start()
    {
        controller = GetComponent<EnemyAIControllerScript>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        animator = GetComponent<Animator>();
        healthSystem = new HealthSystem(maxHP);
        Player.punchEvent.AddListener(OnHaveBeenPunched);
        Player.kickEvent.AddListener(OnHaveBeenKicked);
    }

    void Update()
    {
    }

    private void OnHaveBeenPunched()
    {
        if (playerCanHit)
        {
            animator.SetTrigger("IsPunched");
        }
    }

    private void OnHaveBeenKicked(float force)
    {
        if (playerCanHit)
        {
            animator.SetTrigger("IsKicked");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("HitArea"))
        {
            playerCanHit = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("HitArea"))
        {
            playerCanHit = false;
        }
    }
}
