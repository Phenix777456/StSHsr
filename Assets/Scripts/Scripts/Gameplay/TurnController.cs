using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TurnController : MonoBehaviour
{
    [Header("Field")]
    [SerializeField] private Vector2Int _boardSize = new Vector2Int(8, 8);
    [SerializeField] private Vector3 _origin = Vector3.zero;
    [SerializeField, Min(0.01f)] private float _cellSize = 1f;

    [Header("Player")]
    [SerializeField] private Vector2Int _startCell = Vector2Int.zero;

    [Header("Scene references")]
    [SerializeField] private BoardHighlighter _boardHighlighter;
    [SerializeField] private PlayerActor _playerActor;
    [SerializeField] private EnemySpawner _enemySpawner;

    private readonly HashSet<Enemy> _movingEnemies = new HashSet<Enemy>();

    private IPlayerActor _player;
    private bool _isPlayerMoving;

    private void Awake()
    {
        _player = _playerActor;
    }

    private void OnEnable()
    {
        _player.RouteStarted += OnPlayerRouteStarted;
        _player.TurnFinished += OnPlayerTurnFinished;
    }

    private void OnDisable()
    {
        _player.RouteStarted -= OnPlayerRouteStarted;
        _player.TurnFinished -= OnPlayerTurnFinished;
        StopAllCoroutines();
    }

    private void Start()
    {
        var board = new Board(_boardSize);
        var mapper = new GridMapper(_origin, _cellSize);

        _boardHighlighter.Build(_boardSize, mapper);
        _playerActor.Initialize(board, mapper, _startCell);
        _enemySpawner.Initialize(board, mapper);
        _enemySpawner.SpawnInitial();

        BeginRound();
    }

    private void BeginRound()
    {
        foreach (Enemy enemy in _enemySpawner.Active)
        {
            enemy.PlanTurn();
        }

        _player.BeginTurn();
    }

    private void OnPlayerRouteStarted()
    {
        var enemies = new List<Enemy>(_enemySpawner.Active);

        _isPlayerMoving = true;
        _movingEnemies.Clear();

        foreach (Enemy enemy in enemies)
        {
            enemy.TurnFinished += OnEnemyTurnFinished;
            _movingEnemies.Add(enemy);

            if (!enemy.TryStartMove())
            {
                _movingEnemies.Remove(enemy);
            }
        }

        StartCoroutine(FinishRound(enemies));
    }

    private void OnPlayerTurnFinished()
    {
        _isPlayerMoving = false;
    }

    private void OnEnemyTurnFinished(Enemy enemy)
    {
        _movingEnemies.Remove(enemy);
    }

    private IEnumerator FinishRound(List<Enemy> enemies)
    {
        yield return new WaitUntil(IsMovePhaseFinished);

        foreach (Enemy enemy in enemies)
        {
            enemy.TurnFinished -= OnEnemyTurnFinished;
        }

        BeginRound();
    }

    private bool IsMovePhaseFinished()
    {
        if (_isPlayerMoving)
        {
            return false;
        }

        _movingEnemies.RemoveWhere(enemy => !enemy.IsSpawned);

        return _movingEnemies.Count == 0;
    }
}
