using UnityEngine;

public sealed class Unit
{
    public Unit(Vector2Int position, Team team, IDamageable damageable)
    {
        Position = position;
        Team = team;
        Damageable = damageable;
    }

    public Vector2Int Position { get; internal set; }

    public Team Team { get; }

    public IDamageable Damageable { get; }
}
