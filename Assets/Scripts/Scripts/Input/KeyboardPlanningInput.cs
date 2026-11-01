using System;
using UnityEngine;
using UnityEngine.InputSystem;


    [RequireComponent(typeof(PlayerInput))]
    public sealed class KeyboardPlanningInput : MonoBehaviour, IPlanningInput
    {
        public event Action<Vector2Int> DirectionPressed;
        public event Action UndoPressed;
        public event Action ConfirmPressed;

        private void OnMovement(InputValue value)
        {
            var raw = value.Get<Vector2>();
            var direction = new Vector2Int(Mathf.RoundToInt(raw.x), Mathf.RoundToInt(raw.y));

            bool isSingleAxis = (direction.x != 0) != (direction.y != 0);
            if (!isSingleAxis) return;

            DirectionPressed?.Invoke(direction);
        }

        private void OnConfirm() => ConfirmPressed?.Invoke();

        private void OnUndo() => UndoPressed?.Invoke();
    }

