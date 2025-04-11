using Domain;
using Inputs;
using Presentation;
using Samples.Buttons;
using Samples.Data;
using UnityEngine;

namespace Samples
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
            var carousel = new Carousel(config, items, 0);
            
            var viewAnimated = go.AddComponent<CarouselPresenterAnimated>();
            viewAnimated.Initialize(carousel);
            
            var detector = FindFirstObjectByType<SwipeAndDragDetector>();
            var inputListener = go.AddComponent<CarouselInputListener>();
            inputListener.SetSwipeAndDragDetector(detector);
        }
    }
}