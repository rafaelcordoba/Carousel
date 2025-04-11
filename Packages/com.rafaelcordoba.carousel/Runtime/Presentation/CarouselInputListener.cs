using Inputs;
using UnityEngine;

namespace Presentation
{
    public class CarouselInputListener : MonoBehaviour 
    {
        private SwipeAndDragDetector _swipeAndDragDetector;
        private CarouselPresenter _carouselPresenter;

        private void Awake() => 
            _carouselPresenter = GetComponent<CarouselPresenter>();

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
                _carouselPresenter.Model.Next();
            else
                _carouselPresenter.Model.Prev();
        }
    }
}