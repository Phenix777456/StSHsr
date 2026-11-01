using System;

public interface IDamageable
{
    bool IsAlive { get; }

    event Action Died;

    void TakeDamage(DamageInfo damage);
}
