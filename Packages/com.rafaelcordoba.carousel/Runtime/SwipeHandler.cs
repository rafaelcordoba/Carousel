using System;
using UnityEngine;

namespace Carousel.Runtime
{
    [RequireComponent(typeof(CarouselStatic))]
    public class SwipeHandler : MonoBehaviour
    {
        [SerializeField] private float swipeSensitivity = 0.003f;
        [SerializeField] private float minDistanceForSwipeWhileDragging = 0.7f;
        [SerializeField] private float minDistanceForSwipe = 0.5f;
        
        private CarouselStatic _carousel;
        private bool _isDragging;
        private Vector2 _lastTouchPosition;
        private float _swipeDistance;

        private enum InputState { None, Began, Moved, Ended }

        private void Awake()
        {
            _carousel = GetComponent<CarouselStatic>();
            if (!_carousel)
                throw new Exception("CarouselStatic component not found on the GameObject.");
        }

        public void Update()
        {
            if (!_carousel)
                return;

            var (currentState, currentPosition) = GetInputStateAndPosition();

            switch (currentState)
            {
                case InputState.Began:
                    StartDragging(currentPosition);
                    break;
                case InputState.Moved:
                    if (_isDragging) 
                        UpdateDragging(currentPosition);
                    break;
                case InputState.Ended:
                    if (_isDragging) 
                        EndDragging();
                    break;
                case InputState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private (InputState state, Vector2 position) GetInputStateAndPosition()
        {
            if (TryGetTouchInput(out var currentState, out var currentPosition))
            {
                return (currentState, currentPosition);
            }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL 
            return TryGetMouseInput(out currentState, out currentPosition) ? 
                (currentState, currentPosition) : 
                (InputState.None, Vector2.zero);
#endif
        }

        private static bool TryGetTouchInput(out InputState state, out Vector2 position)
        {
            state = InputState.None;
            position = Vector2.zero;

            if (Input.touchCount <= 0) 
                return false;
            
            var touch = Input.GetTouch(0);
            position = touch.position;
            state = touch.phase switch
            {
                TouchPhase.Began => InputState.Began,
                TouchPhase.Moved => InputState.Moved,
                TouchPhase.Ended or TouchPhase.Canceled => InputState.Ended,
                _ => state
            };
            return true;
        }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        private static bool TryGetMouseInput(out InputState state, out Vector2 position)
        {
            state = InputState.None;
            position = Vector2.zero;

            if (Input.GetMouseButtonDown(0))
            {
                state = InputState.Began;
                position = Input.mousePosition;
                return true;
            }

            if (Input.GetMouseButton(0))
            {
                state = InputState.Moved;
                position = Input.mousePosition;
                return true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                state = InputState.Ended;
                return true;
            }

            return false; 
        }
#endif

        private void StartDragging(Vector2 position)
        {
            _isDragging = true;
            _lastTouchPosition = position;
            _swipeDistance = 0f;
        }

        private void UpdateDragging(Vector2 currentPosition)
        {
            var deltaX = currentPosition.x - _lastTouchPosition.x;
            _swipeDistance += deltaX * swipeSensitivity;

            if (Mathf.Abs(_swipeDistance) >= minDistanceForSwipeWhileDragging)
            {
                var indexChange = Mathf.FloorToInt(_swipeDistance);
                var targetIndex = _carousel.SelectedIndex - indexChange;
                targetIndex = Mathf.Clamp(targetIndex, 0, _carousel.DataCount - 1);

                if (targetIndex != _carousel.SelectedIndex)
                {
                    _carousel.Select(targetIndex);
                    _swipeDistance -= indexChange;
                }
            }

            _lastTouchPosition = currentPosition;
        }

        private void EndDragging()
        {
            _isDragging = false;

            if (Mathf.Abs(_swipeDistance) > minDistanceForSwipe)
            {
                var indexChange = Mathf.RoundToInt(_swipeDistance);
                var targetIndex = Mathf.Clamp(_carousel.SelectedIndex - indexChange, 0, _carousel.DataCount - 1);
                if (targetIndex != _carousel.SelectedIndex) _carousel.Select(targetIndex);
            }

            _swipeDistance = 0f;
        }
    }
}