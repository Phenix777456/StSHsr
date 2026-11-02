using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool _pool;
    [SerializeField] private BoardHighlighter _highlighter;
    [SerializeField] private Vector2Int[] _startCells;

    private readonly List<Enemy> _active = new List<Enemy>();

    private Board _board;
    private GridMapper _mapper;

    public IReadOnlyList<Enemy> Active => _active;

    public void Initialize(Board board, GridMapper mapper)
    {
        _board = board;
        _mapper = mapper;
    }

    public void SpawnInitial()
    {
        foreach (Vector2Int cell in _startCells)
        {
            if (!TrySpawn(cell, out _))
            {
                Debug.LogWarning("Cannot spawn enemy at " + cell, this);
            }
        }
    }

    public bool TrySpawn(Vector2Int cell, out Enemy enemy)
    {
        if (_board == null)
        {
            throw new InvalidOperationException("Spawner is not initialized.");
        }

        enemy = null;

        if (!_board.IsWalkable(cell))
        {
            return false;
        }

        Enemy instance = _pool.Get();

        try
        {
            instance.Spawn(_board, _mapper, _highlighter, cell);
            instance.gameObject.SetActive(true);
        }
        catch
        {
            _pool.Release(instance);

            throw;
        }

        _active.Add(instance);
        instance.DespawnRequested += OnEnemyDespawnRequested;
        enemy = instance;

        return true;
    }

    private void OnEnemyDespawnRequested(Enemy enemy)
    {
        enemy.DespawnRequested -= OnEnemyDespawnRequested;
        _active.Remove(enemy);
    }
}
