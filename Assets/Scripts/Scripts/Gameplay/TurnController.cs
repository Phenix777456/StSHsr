using System;
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

    private IPlayerActor _player;

    private void Awake()
    {
        _player = _playerActor;
    }

    private void OnEnable()
    {
        _player.TurnFinished += OnPlayerTurnFinished;
    }

    private void OnDisable()
    {
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

        _player.BeginTurn();
    }

    private void OnPlayerTurnFinished()
    {
        StartCoroutine(EnemyPhase());
    }

    private IEnumerator EnemyPhase()
    {
        var enemies = new List<Enemy>(_enemySpawner.Active);

        foreach (Enemy enemy in enemies)
        {
            yield return RunEnemyTurn(enemy);
        }

        _player.BeginTurn();
    }

    private static IEnumerator RunEnemyTurn(Enemy enemy)
    {
        bool isFinished = false;
        Action onFinished = () => isFinished = true;

        enemy.TurnFinished += onFinished;
        enemy.BeginTurn();

        yield return new WaitUntil(() => isFinished || !enemy.IsSpawned);

        enemy.TurnFinished -= onFinished;
    }
}
