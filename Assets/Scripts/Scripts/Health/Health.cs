using System;
using UnityEngine;

public sealed class Health : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1)] private int _maxHealth = 3;

    public int Current { get; private set; }

    public int Max
    {
        get
        {
            return _maxHealth;
        }
    }

    public bool IsAlive
    {
        get
        {
            return Current > 0;
        }
    }

    public event Action<int, int> Changed;

    public event Action Died;

    private void Awake()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        Current = _maxHealth;
        Changed?.Invoke(Current, Max);
    }

    public void TakeDamage(DamageInfo damage)
    {
        if (!IsAlive || damage.Amount <= 0)
        {
            return;
        }

        Current = Mathf.Max(0, Current - damage.Amount);
        Changed?.Invoke(Current, Max);

        if (!IsAlive)
        {
            Died?.Invoke();
        }
    }
}
