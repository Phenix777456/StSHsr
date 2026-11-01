using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathMover), typeof(Health))]
public sealed class PlayerActor : MonoBehaviour, IPlayerActor
{
    [SerializeField] private RoutePlanningSession _planningSession;
    [SerializeField, Min(1)] private int _maxSteps = 4;
    [SerializeField, Min(1)] private int _damage = 1;

    private Health _health;
    private PathMover _mover;
    private PlayerAnimator _animator;
    private Board _board;
    private GridMapper _mapper;
    private Unit _unit;
    private StepResolver _stepResolver;
    private bool _isTurnActive;

    public event Action TurnFinished;

    public Vector2Int Cell => _unit.Position;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _mover = GetComponent<PathMover>();
        _animator = GetComponent<PlayerAnimator>();
    }

    private void OnEnable()
    {
        _planningSession.RouteConfirmed += OnRouteConfirmed;
        _health.Died += OnDied;
    }

    private void OnDisable()
    {
        _planningSession.RouteConfirmed -= OnRouteConfirmed;
        _health.Died -= OnDied;
        _planningSession.End();

        StopAllCoroutines();
        _isTurnActive = false;
    }

    public void Initialize(Board board, GridMapper mapper, Vector2Int startCell)
    {
        if (_unit != null)
        {
            throw new InvalidOperationException("Player is already initialized.");
        }

        _unit = new Unit(startCell, Team.Player, _health);

        if (!board.TryPlace(_unit, startCell))
        {
            throw new InvalidOperationException("Player start cell is unavailable.");
        }

        _board = board;
        _mapper = mapper;
        _stepResolver = new StepResolver(board);
        transform.position = mapper.CellToWorld(startCell);
    }

    public void BeginTurn()
    {
        if (_unit == null || !_health.IsAlive || _isTurnActive)
        {
            return;
        }

        _isTurnActive = true;
        _planningSession.Begin(_board, _mapper, Cell, _maxSteps);
    }

    private void OnRouteConfirmed(IReadOnlyList<Vector2Int> route)
    {
        if (!_isTurnActive || !_health.IsAlive)
        {
            return;
        }

        StartCoroutine(ExecuteRoute(new List<Vector2Int>(route)));
    }

    private IEnumerator ExecuteRoute(IReadOnlyList<Vector2Int> route)
    {
        if (route.Count > 0 && route[0] == Cell)
        {
            yield return FollowRoute(route);
        }

        _isTurnActive = false;

        if (_health.IsAlive)
        {
            TurnFinished?.Invoke();
        }
    }

    private IEnumerator FollowRoute(IReadOnlyList<Vector2Int> route)
    {
        for (int i = 1; i < route.Count; i++)
        {
            if (!_health.IsAlive)
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
                yield return Attack(occupant, target - Cell);

                if (occupant.Damageable.IsAlive)
                {
                    yield break;
                }

                _board.TryRemove(occupant);
            }

            if (!_board.TryMove(_unit, target))
            {
                yield break;
            }

            yield return _mover.MoveTo(_mapper.CellToWorld(target));
        }
    }

    private IEnumerator Attack(Unit target, Vector2Int direction)
    {
        yield return null;

        target.Damageable.TakeDamage(new DamageInfo(_damage));
    }

    private void OnDied()
    {
        _board.TryRemove(_unit);
        _planningSession.End();
        gameObject.SetActive(false);
    }
}
