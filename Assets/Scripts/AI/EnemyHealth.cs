using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    private HealthSystem healthSystem;

    private HealthBar hpBar;

    [Range(0, 500)] [SerializeField] private int maxHealth;

    private void Awake()
    {
        healthSystem = new HealthSystem(maxHealth);
        healthSystem.OnDead += HealthSystem_OnDead;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        hpBar = gameObject.GetComponentInChildren<HealthBar>();
    }

    public void Damage(float damage)
    {
        healthSystem.Damage(damage);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        Debug.Log("EnemyDead");
        Destroy(gameObject);

    }
    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        hpBar.ChangeHealthPercent(healthSystem.GetHealthPercent());
    }
}
