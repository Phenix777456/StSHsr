using System;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(EnemyView))]
public abstract class Enemy : MonoBehaviour
{
    private Health _health;
    private Board _board;
    private Unit _unit;
    private bool _isPrepared;

    public event Action<Enemy> DespawnRequested;

    public bool IsSpawned { get; private set; }

    public Vector2Int Cell
    {
        get
        {
            if (_unit == null)
            {
                throw new InvalidOperationException("Enemy is not spawned.");
            }

            return _unit.Position;
        }
    }

    protected EnemyView View { get; private set; }

    protected EnemyBehaviour Behaviour { get; private set; }

    protected virtual void Awake()
    {
        Prepare();
    }

    public void Spawn(Board board, GridMapper mapper, Vector2Int cell)
    {
        Prepare();

        if (IsSpawned)
        {
            throw new InvalidOperationException("Enemy is already spawned.");
        }

        _health.ResetHealth();

        var unit = new Unit(cell, Team.Enemy, _health);

        if (!board.TryPlace(unit, cell))
        {
            throw new InvalidOperationException("Spawn cell is unavailable.");
        }

        _board = board;
        _unit = unit;
        Behaviour = CreateBehaviour();
        transform.position = mapper.CellToWorld(cell);
        IsSpawned = true;
        _health.Died += OnDied;
    }

    public void Despawn()
    {
        if (!IsSpawned)
        {
            return;
        }

        IsSpawned = false;
        _health.Died -= OnDied;
        _board.TryRemove(_unit);
        _board = null;
        _unit = null;
        Behaviour = null;
    }

    protected abstract EnemyBehaviour CreateBehaviour();

    protected virtual void OnDied()
    {
        DespawnRequested?.Invoke(this);
    }

    private void Prepare()
    {
        if (_isPrepared)
        {
            return;
        }

        _health = GetComponent<Health>();
        View = GetComponent<EnemyView>();
        _isPrepared = true;
    }
}
