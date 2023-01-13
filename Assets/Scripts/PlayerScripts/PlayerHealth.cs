using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    private HealthSystem healthSystem;

    [Range(0, 500)] [SerializeField] private int maxHealth;

    private Image playerHPBarImage;

    private void Awake()
    {
        healthSystem = new HealthSystem(maxHealth);
        healthSystem.OnDead += HealthSystem_OnDead;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnHealed += HealthSystem_OnHealed;
    }

    private void Start()
    {
        playerHPBarImage = UIService.Current.GetPlayerHpBarImage();
    }

    public void Damage(float damage)
    {
        healthSystem.Damage(damage);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        Debug.Log("PlayerDead");
        UIService.Current.ShowDeathMessage();
    }
    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        playerHPBarImage.fillAmount = healthSystem.GetHealthPercent();
    }
    private void HealthSystem_OnHealed(object sender, EventArgs e)
    {
        Debug.Log("PlayerHealed");
    }
}
