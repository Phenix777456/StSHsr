using System;
using UnityEngine;

public interface IPlanningInput
{
        event Action<Vector2Int> DirectionPressed;
        event Action UndoPressed;
        event Action ConfirmPressed;
}


