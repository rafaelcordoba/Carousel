using Carousel.Runtime;
using Carousel.Samples.Buttons;
using Carousel.Samples.Data;
using UnityEngine;

namespace Carousel.Samples
{
    public class SampleCarouselsPresenter : MonoBehaviour
    {
        [SerializeField] private Transform container;
        
        private LoadButton[] _buttons;

        private void Awake()
        {
            _buttons = GetComponentsInChildren<LoadButton>();
            foreach (var button in _buttons) 
                button.SetPresenter(this);
        }

        public void LoadCarousel(CarouselConfig config, SampleData data)
        {
            container.DestroyChildren();
            
            var go = new GameObject("Carousel");
            var carousel = go.AddComponent<Carousel3D>();
            var items = data.ToCarouselItems();
            carousel.Initialize(config, items, 0);
            go.AddComponent<SwipeInputHandler>().Initialize(carousel);
            go.transform.SetParent(container, false);
        }
    }
}