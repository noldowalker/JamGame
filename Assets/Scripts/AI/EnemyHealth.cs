using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour, IKickable, IPunchable
{
    private HealthSystem healthSystem;
    private AudioSource audioSource;
    private EnemyAIControllerScript enemyAI;
    private HealthBar hpBar;

    [Range(0, 500)] [SerializeField] private int maxHealth;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        enemyAI = GetComponent<EnemyAIControllerScript>();
        healthSystem = new HealthSystem(maxHealth);
        healthSystem.OnDead += HealthSystem_OnDead;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        hpBar = gameObject.GetComponentInChildren<HealthBar>();
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        // Debug.Log("EnemyDead");
        SoundHandleScript.Current.PlaySound(SoundEnum.ENEMY_DEATH, audioSource);
        animator.SetTrigger("isDie");
        Destroy(gameObject, 4.5f);

    }
    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        hpBar.ChangeHealthPercent(healthSystem.GetHealthPercent());

        if (enemyAI.enemyType == EnemyType.KNIGHT || enemyAI.enemyType == EnemyType.ROYAL_KNIGHT)
        {
            SoundHandleScript.Current.PlaySound(SoundEnum.HIT_REACTION_ARMOR, audioSource);
        } else
        {
            SoundHandleScript.Current.PlaySound(SoundEnum.HIT_REACTION, audioSource);
        }
    }

    public void Kick(float damage, float force, Vector3 direction)
    {
        animator.SetTrigger("IsKicked");
        healthSystem.Damage(damage);
    }

    public void Punch(float damage)
    {
        animator.SetTrigger("IsPunched");
        healthSystem.Damage(damage);
    }
}
