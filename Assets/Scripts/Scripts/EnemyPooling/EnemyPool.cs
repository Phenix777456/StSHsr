public sealed class EnemyPool : PoolBase<Enemy>
{
    public Enemy Get()
    {
        return Pool.Get();
    }

    public void Release(Enemy enemy)
    {
        Pool.Release(enemy);
    }

    protected override void ActionOnGet(Enemy enemy)
    {
        base.ActionOnGet(enemy);
        enemy.DespawnRequested += Release;
    }

    protected override void ActionOnRelease(Enemy enemy)
    {
        enemy.DespawnRequested -= Release;
        enemy.Despawn();
        base.ActionOnRelease(enemy);
    }
}
