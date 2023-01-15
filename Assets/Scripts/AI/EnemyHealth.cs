using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameLogic;

[RequireComponent(typeof(EnemyAIControllerScript))]
public class EnemyHealth : MonoBehaviour, IKickable, IPunchable, IStompable
{
    [SerializeField] private Transform pfVFXpunch;
    [SerializeField] private Transform pfVFXkick;
    [SerializeField] private Transform pfVFXstomp;

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

        if (aiSystem.EnemyType == EnemyType.KNIGHT || aiSystem.EnemyType == EnemyType.ROYAL_KNIGHT)
        {
            SoundHandleScript.Current.PlaySound(SoundEnum.HIT_REACTION_ARMOR, audioSource);
        } else
        {
            SoundHandleScript.Current.PlaySound(SoundEnum.HIT_REACTION, audioSource);
        }
    }

    public void Kick(float damage, float force, Vector3 direction)
    {
        Instantiate(pfVFXkick, transform.position, transform.rotation);
        animator.SetTrigger("IsKicked");
        healthSystem.Damage(damage);
        aiSystem.ReactOnKick();
    }

    public void Punch(float damage)
    {
        Instantiate(pfVFXpunch, transform.position, transform.rotation);
        animator.SetTrigger("IsPunched");
        healthSystem.Damage(damage);
        aiSystem.ReactOnPunch();

    }

    public void Stomp(float damage)
    {
        if (aiSystem.EnemyType == EnemyType.KING)
            return;

        Instantiate(pfVFXstomp, transform.position, pfVFXstomp.rotation);
        healthSystem.Damage(damage);
    }
}
