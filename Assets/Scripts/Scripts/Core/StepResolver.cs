using UnityEngine;

public sealed class StepResolver
{
    private readonly IBoardView _board;

    public StepResolver(IBoardView board)
    {
        _board = board;
    }

    public StepKind Resolve(Unit actor, Vector2Int target, out Unit occupant)
    {
        occupant = null;

        if (!IsAdjacent(actor.Position, target) || !_board.InBounds(target))
        {
            return StepKind.Blocked;
        }

        occupant = _board.GetUnit(target);

        if (occupant == null)
        {
            return StepKind.Move;
        }

        return occupant.Team == actor.Team ? StepKind.Blocked : StepKind.Attack;
    }

    private static bool IsAdjacent(Vector2Int from, Vector2Int to)
    {
        Vector2Int offset = to - from;

        return Mathf.Abs(offset.x) + Mathf.Abs(offset.y) == 1;
    }
}
