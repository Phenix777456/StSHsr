using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(EnemyView))]
[RequireComponent(typeof(PathMover), typeof(EnemyRoutePlanningSession))]
public abstract class Enemy : MonoBehaviour
{
    [SerializeField, Min(1)] private int _damage = 1;

    private Health _health;
    private PathMover _mover;
    private EnemyRoutePlanningSession _planning;
    private Board _board;
    private GridMapper _mapper;
    private BoardHighlighter _highlighter;
    private StepResolver _stepResolver;
    private Unit _unit;
    private IReadOnlyList<Vector2Int> _plannedRoute;
    private bool _isPrepared;
    private bool _isMoving;

    public event Action<Enemy> DespawnRequested;
    public event Action<Enemy> TurnFinished;

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

    private void OnDisable()
    {
        StopAllCoroutines();
        _isMoving = false;
    }

    public void Spawn(Board board, GridMapper mapper, BoardHighlighter highlighter, Vector2Int cell)
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
        _mapper = mapper;
        _highlighter = highlighter;
        _stepResolver = new StepResolver(board);
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
        _isMoving = false;
        _plannedRoute = null;
        _planning.Clear();
        _health.Died -= OnDied;
        _board.TryRemove(_unit);
        _board = null;
        _mapper = null;
        _highlighter = null;
        _stepResolver = null;
        _unit = null;
        Behaviour = null;
    }

    public void PlanTurn()
    {
        if (!IsSpawned || _isMoving)
        {
            return;
        }

        _plannedRoute = _planning.Plan(_board, _mapper, _highlighter, Cell, Behaviour.Steps);
    }

    public bool TryStartMove()
    {
        if (!IsSpawned || _isMoving || _plannedRoute == null)
        {
            return false;
        }

        _isMoving = true;
        _planning.Clear();
        StartCoroutine(Move(_plannedRoute));
        _plannedRoute = null;

        return true;
    }

    protected abstract EnemyBehaviour CreateBehaviour();

    protected virtual void OnDied()
    {
        DespawnRequested?.Invoke(this);
    }

    private IEnumerator Move(IReadOnlyList<Vector2Int> route)
    {
        yield return FollowRoute(route);

        _isMoving = false;

        if (IsSpawned)
        {
            TurnFinished?.Invoke(this);
        }
    }

    private IEnumerator FollowRoute(IReadOnlyList<Vector2Int> route)
    {
        for (int i = 1; i < route.Count; i++)
        {
            if (!IsSpawned)
            {
                yield break;
            }

            Vector2Int target = route[i];
            StepKind step = _stepResolver.Resolve(_unit, target, out Unit occupant);

            if (step == StepKind.Blocked)
            {
                yield break;
            }

            if (step == StepKind.Attack)
            {
                occupant.Damageable.TakeDamage(new DamageInfo(_damage));

                yield break;
            }

            if (!_board.TryMove(_unit, target))
            {
                yield break;
            }

            yield return _mover.MoveTo(_mapper.CellToWorld(target));
        }
    }

    private void Prepare()
    {
        if (_isPrepared)
        {
            return;
        }

        _health = GetComponent<Health>();
        _mover = GetComponent<PathMover>();
        _planning = GetComponent<EnemyRoutePlanningSession>();
        View = GetComponent<EnemyView>();
        _isPrepared = true;
    }
}
