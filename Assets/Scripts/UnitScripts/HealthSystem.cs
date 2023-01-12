using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    [SerializeField] private int maximumHealth = 100;

    private int health;

    private void Awake()
    {
        health = maximumHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        OnDamaged?.Invoke(this, EventArgs.Empty);

        if (health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return (float)health / maximumHealth;
    }
}
