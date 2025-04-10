using Carousel.Runtime;
using Carousel.Samples.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Carousel.Samples.Buttons
{
    public class LoadButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private CarouselConfig config;
        [SerializeField] private SampleData data;
        private SampleCarouselsPresenter _presenter;

        private void Awake() => button.onClick.AddListener(OnButtonClicked);
        private void OnDestroy() => button.onClick.RemoveListener(OnButtonClicked);
        private void OnButtonClicked() => _presenter.LoadCarousel(config, data);
        public void SetPresenter(SampleCarouselsPresenter presenter) => _presenter = presenter;
    }
}