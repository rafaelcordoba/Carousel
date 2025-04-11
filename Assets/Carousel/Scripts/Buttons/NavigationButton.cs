using System;
using Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace Carousel.Samples.Buttons
{
    [RequireComponent(typeof(Button))]
    public class NavigationButton : MonoBehaviour
    {
        private enum NavigationType { Next, Previous, First, Last }
        [SerializeField] private NavigationType navigationType;
        
        private CarouselViewStatic _carouselView;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            _carouselView = FindObjectOfType<CarouselViewStatic>();
            if (!_carouselView)
            {
                Debug.LogError("Carousel3D not found");
                return;
            }
            
            Action action = navigationType switch
            {
                NavigationType.Next => _carouselView.Next,
                NavigationType.Previous => _carouselView.Prev,
                NavigationType.First => _carouselView.First,
                NavigationType.Last => _carouselView.Last,
                _ => throw new ArgumentOutOfRangeException(nameof(navigationType))
            };
            action();
        }
    }
}