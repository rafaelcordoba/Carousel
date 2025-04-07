using UnityEngine;

namespace Carousel.Runtime
{
    public class SwipeInputHandler : MonoBehaviour
    {
        [SerializeField] private float swipeSensitivity = 0.003f;
        [SerializeField] private float minDistanceForSwipeWhileDragging = 0.7f;
        [SerializeField] private float minDistanceForSwipe = 0.5f;
        private Carousel3D _carousel;

        private bool _isDragging;
        private Vector2 _lastTouchPosition;
        private float _swipeDistance;

        public void Update()
        {
            if (!_carousel)
                return;

            if (Input.touchCount > 0)
                HandleTouchInput(Input.GetTouch(0));

#if UNITY_EDITOR
            HandleMouseInput();
#endif
        }

        public void Initialize(Carousel3D carousel)
        {
            _carousel = carousel;
        }

        private void HandleTouchInput(Touch touch)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartDragging(touch.position);
                    break;

                case TouchPhase.Moved:
                    if (_isDragging) UpdateDragging(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    EndDragging();
                    break;
            }
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
                StartDragging(Input.mousePosition);
            else if (Input.GetMouseButton(0) && _isDragging)
                UpdateDragging(Input.mousePosition);
            else if (Input.GetMouseButtonUp(0) && _isDragging) EndDragging();
        }

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