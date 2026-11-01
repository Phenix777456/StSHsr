using UnityEngine;


    public sealed class GridMapper
    {
        private readonly Vector3 _origin;
        private readonly float _cellSize;

        public GridMapper(Vector3 origin, float cellSize)
        {
            _origin = origin;
            _cellSize = cellSize;
        }

        public Vector3 CellToWorld(Vector2Int cell) =>
            _origin + new Vector3(cell.x + 0.5f, cell.y + 0.5f, 0f) * _cellSize;

        public Vector2Int WorldToCell(Vector3 world)
        {
            var local = (world - _origin) / _cellSize;
            return new Vector2Int(Mathf.FloorToInt(local.x), Mathf.FloorToInt(local.y));
        }
    }

