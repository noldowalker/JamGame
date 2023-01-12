using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour, IKickable, IPunchable
{
    private HealthSystem healthSystem;

    private HealthBar hpBar;

    [Range(0, 500)] [SerializeField] private int maxHealth;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        healthSystem = new HealthSystem(maxHealth);
        healthSystem.OnDead += HealthSystem_OnDead;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        hpBar = gameObject.GetComponentInChildren<HealthBar>();
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
       // Debug.Log("EnemyDead");
        Destroy(gameObject);

    }
    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        hpBar.ChangeHealthPercent(healthSystem.GetHealthPercent());
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
