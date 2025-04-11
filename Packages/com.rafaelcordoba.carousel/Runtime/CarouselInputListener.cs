using UnityEngine;

namespace Carousel.Runtime
{
    public class CarouselInputListener : MonoBehaviour 
    {
        private SwipeAndDragDetector _swipeAndDragDetector;
        private CarouselStatic _carousel;

        private void Awake() => 
            _carousel = GetComponent<CarouselStatic>();

        public void SetSwipeAndDragDetector(SwipeAndDragDetector detector)
        {
            _swipeAndDragDetector = detector;
            _swipeAndDragDetector.OnSwipe.AddListener(HandleDirection);
            _swipeAndDragDetector.OnDragging.AddListener(HandleDirection);
        }

        private void OnDestroy()
        {
            _swipeAndDragDetector.OnSwipe.RemoveListener(HandleDirection);
            _swipeAndDragDetector.OnDragging.RemoveListener(HandleDirection);
        }
        
        private void HandleDirection(Vector2 swipeDirection)
        {
            if (swipeDirection.x < 0)
                _carousel.Next();
            else
                _carousel.Prev();
        }
    }
}