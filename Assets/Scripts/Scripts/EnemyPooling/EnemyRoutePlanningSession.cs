using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyRoutePlanningSession : MonoBehaviour
{
    [SerializeField] private RouteCursorView _cursor;
    [SerializeField, Min(0f)] private float _stepDelay = 0.25f;
    [SerializeField, Min(0f)] private float _confirmDelay = 0.3f;

    private void OnDisable() => _cursor.Hide();

    public IEnumerator Plan(
        IBoardView board,
        GridMapper mapper,
        BoardHighlighter highlighter,
        Vector2Int start,
        IReadOnlyList<Vector2Int> steps,
        Action<IReadOnlyList<Vector2Int>> onConfirmed)
    {
        var planner = new RoutePlanner(board, start, steps.Count);
        planner.RouteChanged += route => highlighter.SetRoute(route);

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

        _cursor.Hide();
        highlighter.ClearRoute();

        onConfirmed(new List<Vector2Int>(planner.Route));
    }
}
