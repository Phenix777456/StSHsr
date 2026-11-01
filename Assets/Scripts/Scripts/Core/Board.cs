using System;
using UnityEngine;

public sealed class Board : IBoardView
{
    private readonly Unit[,] _cells;

    public Board(Vector2Int size)
    {
        if (size.x <= 0 || size.y <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        Size = size;
        _cells = new Unit[size.x, size.y];
    }

    public event Action<Unit, Vector2Int> UnitPlaced;

    public event Action<Unit, Vector2Int, Vector2Int> UnitMoved;

    public event Action<Unit, Vector2Int> UnitRemoved;

    public Vector2Int Size { get; }

    public bool InBounds(Vector2Int cell)
    {
        return cell.x >= 0
            && cell.y >= 0
            && cell.x < Size.x
            && cell.y < Size.y;
    }

    public bool IsWalkable(Vector2Int cell)
    {
        return InBounds(cell) && GetUnit(cell) == null;
    }

    public Unit GetUnit(Vector2Int cell)
    {
        if (!InBounds(cell))
        {
            return null;
        }

        return _cells[cell.x, cell.y];
    }

    public bool TryPlace(Unit unit, Vector2Int cell)
    {
        if (unit == null || IsPlaced(unit) || !IsWalkable(cell))
        {
            return false;
        }

        _cells[cell.x, cell.y] = unit;
        unit.Position = cell;
        UnitPlaced?.Invoke(unit, cell);

        return true;
    }

    public bool TryMove(Unit unit, Vector2Int target)
    {
        if (!IsPlaced(unit) || !IsWalkable(target))
        {
            return false;
        }

        Vector2Int from = unit.Position;

        _cells[from.x, from.y] = null;
        _cells[target.x, target.y] = unit;
        unit.Position = target;
        UnitMoved?.Invoke(unit, from, target);

        return true;
    }

    public bool TryRemove(Unit unit)
    {
        if (!IsPlaced(unit))
        {
            return false;
        }

        Vector2Int cell = unit.Position;

        _cells[cell.x, cell.y] = null;
        UnitRemoved?.Invoke(unit, cell);

        return true;
    }

    private bool IsPlaced(Unit unit)
    {
        return unit != null && ReferenceEquals(GetUnit(unit.Position), unit);
    }
}
