using System;
using System.Collections.Generic;
using UnityEngine;


    public sealed class RoutePlanningSession : MonoBehaviour
    {
        [SerializeField] private KeyboardPlanningInput _input;
        [SerializeField] private RouteCursorView _cursor;
        [SerializeField] private BoardHighlighter _highlighter;

        private RoutePlanner _planner;
        private GridMapper _mapper;

        public bool IsActive => _planner != null;

        public event Action<IReadOnlyList<Vector2Int>> RouteConfirmed;

        public void Begin(IBoardView board, GridMapper mapper, Vector2Int start, int maxSteps)
        {
            if (IsActive)
                End();

            _mapper = mapper;
            _planner = new RoutePlanner(board, start, maxSteps);
            _planner.RouteChanged += OnRouteChanged;

            _input.DirectionPressed += OnDirectionPressed;
            _input.UndoPressed += OnUndoPressed;
            _input.ConfirmPressed += OnConfirmPressed;

            _cursor.Show(_mapper.CellToWorld(start));
        }

        public void End()
        {
            if (IsActive == false)
                return;

            _planner.RouteChanged -= OnRouteChanged;

            _input.DirectionPressed -= OnDirectionPressed;
            _input.UndoPressed -= OnUndoPressed;
            _input.ConfirmPressed -= OnConfirmPressed;

            _cursor.Hide();
            _highlighter.ClearRoute();

            _planner = null;
        }

        private void OnDisable() => End();

        private void OnDirectionPressed(Vector2Int direction) => _planner.TryStep(direction);

        private void OnUndoPressed() => _planner.UndoStep();

        private void OnRouteChanged(IReadOnlyList<Vector2Int> route)
        {
            _highlighter.SetRoute(route);
            _cursor.Show(_mapper.CellToWorld(_planner.Current));
        }

        private void OnConfirmPressed()
        {
            if (_planner.StepsCount == 0)
                return;

            var route = new List<Vector2Int>(_planner.Route);
            End();
            RouteConfirmed?.Invoke(route);
        }
    }

