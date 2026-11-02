using System.Collections.Generic;
using UnityEngine;

public sealed class BoardHighlighter : MonoBehaviour
{
    [SerializeField] private Cell _tilePrefab;
    [SerializeField] private Color _defaultColor = new(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color _routeColor = new(0.3f, 0.8f, 0.4f, 1f);
    [SerializeField] private Color _targetColor = new(0.9f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color _enemyRouteColor = new(0.85f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color _enemyTargetColor = new(1f, 0.5f, 0.2f, 1f);

    private static readonly object PlayerOwner = new object();

    private readonly Dictionary<Vector2Int, SpriteRenderer> _tiles = new();
    private readonly Dictionary<object, RouteLayer> _layers = new();
    private readonly List<Vector2Int> _painted = new();

    public void Build(Vector2Int size, GridMapper mapper)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var cell = new Vector2Int(x, y);
                SpriteRenderer tile = Instantiate(_tilePrefab.GetComponent<SpriteRenderer>(), mapper.CellToWorld(cell), Quaternion.identity, transform);
                tile.color = _defaultColor;
                _tiles[cell] = tile;
            }
        }
    }

    public void SetRoute(IReadOnlyList<Vector2Int> route) => SetLayer(PlayerOwner, route, false);

    public void ClearRoute() => ClearLayer(PlayerOwner);

    public void SetEnemyRoute(object owner, IReadOnlyList<Vector2Int> route) => SetLayer(owner, route, true);

    public void ClearRoute(object owner) => ClearLayer(owner);

    private void SetLayer(object owner, IReadOnlyList<Vector2Int> route, bool isEnemy)
    {
        _layers[owner] = new RouteLayer(new List<Vector2Int>(route), isEnemy);
        Redraw();
    }

    private void ClearLayer(object owner)
    {
        if (_layers.Remove(owner))
        {
            Redraw();
        }
    }

    private void Redraw()
    {
        foreach (Vector2Int cell in _painted)
        {
            _tiles[cell].color = _defaultColor;
        }

        _painted.Clear();

        Paint(true);
        Paint(false);
    }

    private void Paint(bool isEnemy)
    {
        foreach (RouteLayer layer in _layers.Values)
        {
            if (layer.IsEnemy != isEnemy)
            {
                continue;
            }

            for (int i = 1; i < layer.Cells.Count; i++)
            {
                bool isTarget = i == layer.Cells.Count - 1;
                Vector2Int cell = layer.Cells[i];

                _tiles[cell].color = GetColor(isEnemy, isTarget);
                _painted.Add(cell);
            }
        }
    }

    private Color GetColor(bool isEnemy, bool isTarget)
    {
        if (isEnemy)
        {
            return isTarget ? _enemyTargetColor : _enemyRouteColor;
        }

        return isTarget ? _targetColor : _routeColor;
    }

    private readonly struct RouteLayer
    {
        public RouteLayer(List<Vector2Int> cells, bool isEnemy)
        {
            Cells = cells;
            IsEnemy = isEnemy;
        }

        public List<Vector2Int> Cells { get; }

        public bool IsEnemy { get; }
    }
}
