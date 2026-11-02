using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyRoutePlanningSession : MonoBehaviour
{
    [SerializeField] private RouteCursorView _cursor;
    [SerializeField, Min(0f)] private float _stepDelay = 0.25f;
    [SerializeField, Min(0f)] private float _confirmDelay = 0.3f;

    private BoardHighlighter _highlighter;
    private Coroutine _routine;

    public bool IsActive => _routine != null;

    public event Action<IReadOnlyList<Vector2Int>> RouteConfirmed;

    public void Begin(
        IBoardView board,
        GridMapper mapper,
        BoardHighlighter highlighter,
        Vector2Int start,
        IReadOnlyList<Vector2Int> steps)
    {
        End();

        _highlighter = highlighter;
        _routine = StartCoroutine(Plan(board, mapper, start, steps));
    }

    public void End()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        _cursor.Hide();
        _highlighter?.ClearRoute();
    }

    private void OnDisable() => End();

    private IEnumerator Plan(IBoardView board, GridMapper mapper, Vector2Int start, IReadOnlyList<Vector2Int> steps)
    {
        var planner = new RoutePlanner(board, start, steps.Count);
        planner.RouteChanged += OnRouteChanged;

        _cursor.Show(mapper.CellToWorld(start));

        foreach (Vector2Int direction in steps)
        {
            yield return new WaitForSeconds(_stepDelay);

            if (!planner.TryStep(direction))
            {
                break;
            }

            _cursor.Show(mapper.CellToWorld(planner.Current));
        }

        yield return new WaitForSeconds(_confirmDelay);

        planner.RouteChanged -= OnRouteChanged;

        var route = new List<Vector2Int>(planner.Route);
        End();
        RouteConfirmed?.Invoke(route);
    }

    private void OnRouteChanged(IReadOnlyList<Vector2Int> route) => _highlighter.SetRoute(route);
}
