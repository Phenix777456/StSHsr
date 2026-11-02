using System;
using UnityEngine;

public interface IPlayerActor
{
    event Action RouteStarted;
    event Action TurnFinished;

    Vector2Int Cell { get; }

    void BeginTurn();
}
