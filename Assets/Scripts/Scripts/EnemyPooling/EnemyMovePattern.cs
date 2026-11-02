using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Move Pattern")]
public sealed class EnemyMovePattern : ScriptableObject
{
    [SerializeField] private Vector2Int[] _steps;

    public IReadOnlyList<Vector2Int> Steps => _steps;
}
