using System;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool _pool;
    [SerializeField] private Vector2Int[] _startCells;

    private Board _board;
    private GridMapper _mapper;

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
            instance.Spawn(_board, _mapper, cell);
            instance.gameObject.SetActive(true);
        }
        catch
        {
            _pool.Release(instance);

            throw;
        }

        enemy = instance;

        return true;
    }
}
