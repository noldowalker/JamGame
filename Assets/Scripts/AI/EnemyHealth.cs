using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameLogic;

[RequireComponent(typeof(EnemyAIControllerScript))]
public class EnemyHealth : MonoBehaviour, IKickable, IPunchable, IStompable, IUppercutable
{
    [SerializeField] private Transform pfVFXpunch;
    [SerializeField] private Transform pfVFXkick;
    [SerializeField] private Transform pfVFXstomp;

    [Range(0, 500)] [SerializeField] private int maxHealth;
    
    private Animator animator;
    private HealthBar hpBar;
    private HealthSystem healthSystem;
    private Rigidbody rigidbody;
    private EnemyAIControllerScript aiSystem;
    private AudioSource audioSource;
    private bool isDead;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        aiSystem = GetComponent<EnemyAIControllerScript>();
        healthSystem = new HealthSystem(maxHealth);
        healthSystem.OnDead += HealthSystem_OnDead;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        hpBar = gameObject.GetComponentInChildren<HealthBar>();
        rigidbody = GetComponent<Rigidbody>();
        isDead = false;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        Debug.Log("DEAD");
        isDead = true;
        aiSystem.ReactOnDeath();
        SoundHandleScript.Current.PlaySound(SoundEnum.ENEMY_DEATH, audioSource);
    }
    
    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        if (!isDead)
        {
            hpBar.ChangeHealthPercent(healthSystem.GetHealthPercent());

            if (aiSystem.EnemyType == EnemyType.KNIGHT || aiSystem.EnemyType == EnemyType.ROYAL_KNIGHT)
            {
                SoundHandleScript.Current.PlaySound(SoundEnum.HIT_REACTION_ARMOR, audioSource);
            }
            else
            {
                SoundHandleScript.Current.PlaySound(SoundEnum.HIT_REACTION, audioSource);
            }
        }
    }

    public void Kick(float damage, float force, Vector3 direction)
    {
        isDead = healthSystem.Damage(damage);
        if (!isDead)
        {
            Instantiate(pfVFXkick, transform.position, transform.rotation);
            aiSystem.ReactOnKick(force,direction);
        }
    }

    public void Uppercut(float damage, float force)
    {
        isDead = healthSystem.Damage(damage);
        if (!isDead)
        {
            Instantiate(pfVFXkick, transform.position, transform.rotation);
            aiSystem.ReactOnUppercut(force);
        }
    }

    public void Punch(float damage)
    {
        isDead = healthSystem.Damage(damage);
        if (!isDead)
        {
            Instantiate(pfVFXpunch, transform.position, transform.rotation);
            aiSystem.ReactOnPunch();
        }
    }

    public void Stomp(float damage)
    {
        isDead = healthSystem.Damage(damage);
        if (!isDead)
        {
            if (aiSystem.EnemyType == EnemyType.KING)
                return;

            Instantiate(pfVFXstomp, transform.position, pfVFXstomp.rotation);
        }
    }
}
