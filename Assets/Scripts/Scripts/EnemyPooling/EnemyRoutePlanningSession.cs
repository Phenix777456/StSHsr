using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyRoutePlanningSession : MonoBehaviour
{
    [SerializeField] private RouteCursorView _cursor;

    private BoardHighlighter _highlighter;

    public IReadOnlyList<Vector2Int> Plan(
        IBoardView board,
        GridMapper mapper,
        BoardHighlighter highlighter,
        Vector2Int start,
        IReadOnlyList<Vector2Int> steps)
    {
        Clear();

        _highlighter = highlighter;

        var planner = new RoutePlanner(board, start, steps.Count);

        foreach (Vector2Int direction in steps)
        {
            if (!planner.TryStep(direction))
            {
                break;
            }
        }

        var route = new List<Vector2Int>(planner.Route);

        _highlighter.SetEnemyRoute(this, route);
        _cursor.Show(mapper.CellToWorld(planner.Current));

        return route;
    }

    public void Clear()
    {
        _cursor.Hide();
        _highlighter?.ClearRoute(this);
    }

    private void OnDisable() => Clear();
}
