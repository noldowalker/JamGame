using System;
public class HealthSystem
{
    public EventHandler OnHealthChanged;
    public EventHandler OnHealthMaxChanged;
    public EventHandler OnDamaged;
    public EventHandler OnHealed;
    public EventHandler OnDead;

    private float _health;
    private float _healthMax;
    public HealthSystem(float healthMax)
    {
        _healthMax = healthMax;
        _health = healthMax;
    }

    public float GetHealth()
    {
        return _health;
    }

    public float IncreaseMaxHealth(float healthPoints)
    {
        if (_health == _healthMax)
        {
            _healthMax += healthPoints;
            _health = _healthMax;
            return _healthMax;
        }
        else
        {
            return _healthMax += healthPoints;
        }
    }

    public float GetHealthMax()
    {
        return _healthMax;
    }

    public float GetHealthPercent()
    {
        return _health / _healthMax;
    }
    public void Damage(float damageAmount)
    {
        _health -= damageAmount;
        OnDamaged?.Invoke(this, EventArgs.Empty);

        if (_health <= 0)
        {
            _health = 0;
            OnDead?.Invoke(this, EventArgs.Empty);
        }

        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Heal(float healAmount)
    {
        _health += healAmount;
        OnHealed?.Invoke(this, EventArgs.Empty);

        if (_health > _healthMax)
            _health = _healthMax;

        OnHealthChanged?.Invoke(this, EventArgs.Empty);

    }
}
