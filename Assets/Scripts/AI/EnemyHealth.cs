using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour, IKickable, IPunchable, IStompable
[RequireComponent(typeof(EnemyAIControllerScript))]
public class EnemyHealth : MonoBehaviour, IKickable, IPunchable
{
    private HealthSystem healthSystem;
    private AudioSource audioSource;
    private EnemyAIControllerScript enemyAI;
    private HealthBar hpBar;

    [SerializeField] private Transform pfVFXpunch;
    [SerializeField] private Transform pfVFXkick;
    [SerializeField] private Transform pfVFXblood;
    [SerializeField] private Transform pfVFXbloodSplash;

    [Range(0, 500)] [SerializeField] private int maxHealth;
    
    private Animator animator;
    private HealthBar hpBar;
    private HealthSystem healthSystem;
    private EnemyAIControllerScript aiSystem;
    private AudioSource audioSource;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        aiSystem = GetComponent<EnemyAIControllerScript>();

        healthSystem = new HealthSystem(maxHealth);
        healthSystem.OnDead += HealthSystem_OnDead;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        hpBar = gameObject.GetComponentInChildren<HealthBar>();
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        aiSystem.ReactOnDeath();
        SoundHandleScript.Current.PlaySound(SoundEnum.ENEMY_DEATH, audioSource);
    }
    
    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        hpBar.ChangeHealthPercent(healthSystem.GetHealthPercent());

        if (aiSystem.enemyType == EnemyType.KNIGHT || aiSystem.enemyType == EnemyType.ROYAL_KNIGHT)
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
        aiSystem.ReactOnKick();
    }

    public void Punch(float damage)
    {
        animator.SetTrigger("IsPunched");
        healthSystem.Damage(damage);
        aiSystem.ReactOnPunch();
    }

    public void Stomp(float damage)
    {
        healthSystem.Damage(damage);
    }
}
