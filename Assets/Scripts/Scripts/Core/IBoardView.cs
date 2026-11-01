using UnityEngine;

public interface IBoardView
{
    Vector2Int Size { get; }

    bool InBounds(Vector2Int cell);

    bool IsWalkable(Vector2Int cell);

    Unit GetUnit(Vector2Int cell);
}
