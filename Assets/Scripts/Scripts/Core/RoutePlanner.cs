using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class RoutePlanner
{
    private readonly List<Vector2Int> _route = new List<Vector2Int>();
    private readonly IBoardView _board;
    private readonly Unit _actor;
    private readonly int _maxSteps;

    public RoutePlanner(IBoardView board, Vector2Int start, int maxSteps)
    {
        _board = board;
        _actor = board.GetUnit(start);
        _maxSteps = maxSteps;
        _route.Add(start);
    }

    public event Action<IReadOnlyList<Vector2Int>> RouteChanged;

    public IReadOnlyList<Vector2Int> Route => _route;

    public Vector2Int Current => _route[_route.Count - 1];

    public int StepsCount => _route.Count - 1;

    public bool TryStep(Vector2Int direction)
    {
        if (IsUnitStep(direction) == false || StepsCount >= _maxSteps)
        {
            return false;
        }

        if (IsRouteEndedByEnemy())
        {
            return false;
        }

        Vector2Int next = Current + direction;

        if (CanEnter(next) == false)
        {
            return false;
        }

        _route.Add(next);
        RouteChanged?.Invoke(_route);

        return true;
    }

    public void UndoStep()
    {
        if (StepsCount == 0)
        {
            return;
        }

        _route.RemoveAt(_route.Count - 1);
        RouteChanged?.Invoke(_route);
    }

    private static bool IsUnitStep(Vector2Int direction)
    {
        return Mathf.Abs(direction.x) + Mathf.Abs(direction.y) == 1;
    }

    private bool IsRouteEndedByEnemy()
    {
        return StepsCount > 0 && _board.GetUnit(Current) != null;
    }

    private bool CanEnter(Vector2Int cell)
    {
        if (!_board.InBounds(cell) || _route.Contains(cell))
        {
            return false;
        }

        Unit occupant = _board.GetUnit(cell);

        if (occupant == null)
        {
            return true;
        }

        return _actor != null && occupant.Team != _actor.Team;
    }
}
