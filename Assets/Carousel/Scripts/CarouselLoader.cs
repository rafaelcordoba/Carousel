using Carousel.Samples.Buttons;
using Carousel.Samples.Data;
using Domain;
using Inputs;
using Presentation;
using UnityEngine;

namespace Carousel.Samples
{
    public class CarouselLoader : MonoBehaviour
    {
        [SerializeField] private Transform container;
        
        private LoadButton[] _buttons;

        private void Awake()
        {
            _buttons = GetComponentsInChildren<LoadButton>();
            foreach (var button in _buttons) 
                button.SetPresenter(this);

            ForceClickFirstButton();
        }

        private void ForceClickFirstButton()
        {
            if (_buttons is not { Length: > 0 }) 
                return;
            var firstButton = _buttons[0];
            firstButton.OnButtonClicked();
        }

        public void LoadCarousel(Config config, SampleData data)
        {
            container.DestroyChildren();
            
            var go = new GameObject("Carousel");
            go.transform.SetParent(container, false);
            
            var items = data.ToCarouselItems();
            var carousel = go.AddComponent<CarouselViewAnimated>();
            carousel.Initialize(config, items, 0);
            
            var detector = FindFirstObjectByType<SwipeAndDragDetector>();
            var inputListener = go.AddComponent<CarouselInputListener>();
            inputListener.SetSwipeAndDragDetector(detector);
            
            
        }
    }
}