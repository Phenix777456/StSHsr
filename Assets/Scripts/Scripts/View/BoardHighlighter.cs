using System.Collections.Generic;
using UnityEngine;


    public sealed class BoardHighlighter : MonoBehaviour
    {
        [SerializeField] private Cell _tilePrefab;
        [SerializeField] private Color _defaultColor = new(0.2f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color _routeColor = new(0.3f, 0.8f, 0.4f, 1f);
        [SerializeField] private Color _targetColor = new(0.9f, 0.8f, 0.2f, 1f);

        private readonly Dictionary<Vector2Int, SpriteRenderer> _tiles = new();
        private readonly List<Vector2Int> _highlighted = new();

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

        public void SetRoute(IReadOnlyList<Vector2Int> route)
        {
            ClearRoute();

            for (int i = 1; i < route.Count; i++)
            {
                bool isTarget = i == route.Count - 1;
                _tiles[route[i]].color = isTarget ? _targetColor : _routeColor;
                _highlighted.Add(route[i]);
            }
        }

        public void ClearRoute()
        {
            foreach (var cell in _highlighted)
                _tiles[cell].color = _defaultColor;

            _highlighted.Clear();
        }
    }

