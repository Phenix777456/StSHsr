using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyBehaviour
{
    private readonly EnemyMovePattern _pattern;

    public EnemyBehaviour(EnemyMovePattern pattern)
    {
        _pattern = pattern;
    }

    public IReadOnlyList<Vector2Int> Steps => _pattern.Steps;
}
