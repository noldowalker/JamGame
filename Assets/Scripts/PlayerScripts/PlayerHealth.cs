using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamagable, IHealable
{
    private HealthSystem healthSystem;
    private AudioSource audioSource;

    [SerializeField] private Transform pfVFXDamage;
    [Range(0, 500)] [SerializeField] private int maxHealth;

    private Image playerHPBarImage;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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
        UIService.Current.ShowDeathMessage();
    }
    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        playerHPBarImage.fillAmount = healthSystem.GetHealthPercent();
        SoundHandleScript.Current.PlaySound(SoundEnum.HIT_REACTION,audioSource);
        Instantiate(pfVFXDamage, transform.position, transform.rotation);
    }
    private void HealthSystem_OnHealed(object sender, EventArgs e)
    {
        playerHPBarImage.fillAmount = healthSystem.GetHealthPercent();
    }

    public void Heal(float healPoints)
    {
        healthSystem.Heal(healPoints);
    }
}
