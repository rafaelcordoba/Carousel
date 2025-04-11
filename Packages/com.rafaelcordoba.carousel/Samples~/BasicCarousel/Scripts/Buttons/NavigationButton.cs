using System;
using Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Buttons
{
    [RequireComponent(typeof(Button))]
    public class NavigationButton : MonoBehaviour
    {
        private enum NavigationType { Next, Previous, First, Last }
        [SerializeField] private NavigationType navigationType;
        
        private CarouselPresenter _carouselPresenter;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            if (_button)
                _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            _carouselPresenter = FindObjectOfType<CarouselPresenter>();
            if (!_carouselPresenter)
            {
                Debug.LogError("Carousel3D not found");
                return;
            }
            
            Action action = navigationType switch
            {
                NavigationType.Next => _carouselPresenter.Model.Next,
                NavigationType.Previous => _carouselPresenter.Model.Prev,
                NavigationType.First => _carouselPresenter.Model.First,
                NavigationType.Last => _carouselPresenter.Model.Last,
                _ => throw new ArgumentOutOfRangeException(nameof(navigationType))
            };
            action();
        }
    }
}