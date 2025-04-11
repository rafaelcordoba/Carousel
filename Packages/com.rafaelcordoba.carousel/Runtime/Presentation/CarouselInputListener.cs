using Inputs;
using UnityEngine;

namespace Presentation
{
    public class CarouselInputListener : MonoBehaviour 
    {
        private SwipeAndDragDetector _swipeAndDragDetector;
        private CarouselViewStatic _carouselView;

        private void Awake() => 
            _carouselView = GetComponent<CarouselViewStatic>();

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
                _carouselView.Next();
            else
                _carouselView.Prev();
        }
    }
}