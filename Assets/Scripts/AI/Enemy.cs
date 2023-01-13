using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   // public float maxHP;
   [SerializeField] EnemyType enemyType;
    private EnemyAIControllerScript controller;
    private Player player;
    private Animator animator;
    private AudioSource audioSource;
   // private HealthSystem healthSystem;
    private bool playerCanHit;
    void Start()
    {
        controller = GetComponent<EnemyAIControllerScript>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
     //   healthSystem = new HealthSystem(maxHP);
      //  Player.punchEvent.AddListener(OnHaveBeenPunched);
     //   Player.kickEvent.AddListener(OnHaveBeenKicked);
    }

    void Update()
    {

    }


    //Sound Events===========
    private void OnStep()
    {
        SoundHandleScript.Current.PlaySound(SoundEnum.ENEMY_STEP,audioSource);
    }

    private void OnGotHit() {
        if (enemyType == EnemyType.KNIGHT || enemyType == EnemyType.ROYAL_KNIGHT)
        {
            SoundHandleScript.Current.PlaySound(SoundEnum.HIT_REACTION_ARMOR, audioSource);
        } else
        {
            SoundHandleScript.Current.PlaySound(SoundEnum.HIT_REACTION, audioSource);
        }
    }
    private void OnPerformHit()
    {
        SoundHandleScript.Current.PlaySound(SoundEnum.WEAPON_SLASH, audioSource);
    }

    private void OnDeath()
    {
        SoundHandleScript.Current.PlaySound(SoundEnum.ENEMY_DEATH, audioSource);
    }
    //========================

    //private void OnHaveBeenPunched()
    //{
    //    if (playerCanHit)
    //    {
    //        animator.SetTrigger("IsPunched");
    //    }
    //}

    //private void OnHaveBeenKicked(float force)
    //{
    //    if (playerCanHit)
    //    {
    //        animator.SetTrigger("IsKicked");
    //    }
    //}


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag.Equals("HitArea"))
    //    {
    //        playerCanHit = true;
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag.Equals("HitArea"))
    //    {
    //        playerCanHit = false;
    //    }
    //}
}
