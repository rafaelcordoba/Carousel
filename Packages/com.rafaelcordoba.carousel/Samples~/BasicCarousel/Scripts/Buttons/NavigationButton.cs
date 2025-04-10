using System;
using Carousel.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Carousel.Samples.Buttons
{
    [RequireComponent(typeof(Button))]
    public class NavigationButton : MonoBehaviour
    {
        private enum NavigationType { Next, Previous, First, Last }
        [SerializeField] private NavigationType navigationType;
        
        private CarouselStatic _carousel;
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
            _carousel = FindObjectOfType<CarouselStatic>();
            if (!_carousel)
            {
                Debug.LogError("Carousel3D not found");
                return;
            }
            
            Action action = navigationType switch
            {
                NavigationType.Next => _carousel.Next,
                NavigationType.Previous => _carousel.Previous,
                NavigationType.First => _carousel.First,
                NavigationType.Last => _carousel.Last,
                _ => throw new ArgumentOutOfRangeException(nameof(navigationType))
            };
            action();
        }
    }
}