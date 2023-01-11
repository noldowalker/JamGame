using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    private HealthSystem healthSystem;

    [Range(0, 500)] [SerializeField] private int maxHealth;


    private void Awake()
    {
        healthSystem = new HealthSystem(maxHealth);
        healthSystem.OnDead += HealthSystem_OnDead;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnHealed += HealthSystem_OnHealed;
    }

    public void Damage(float damage)
    {
        healthSystem.Damage(damage);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        Debug.Log("PlayerDead");
    }
    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        //  Debug.Log("PlayerDamaged" + healthSystem.GetHealth());
    }
    private void HealthSystem_OnHealed(object sender, EventArgs e)
    {
        Debug.Log("PlayerHealed");
    }
}
