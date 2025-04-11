using Domain;
using Samples.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Buttons
{
    public class LoadButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Config config;
        [SerializeField] private SampleData data;
        private CarouselLoader _presenter;

        private void Awake() => button.onClick.AddListener(OnButtonClicked);
        private void OnDestroy() => button.onClick.RemoveListener(OnButtonClicked);
        public void OnButtonClicked() => _presenter.LoadCarousel(config, data);
        public void SetPresenter(CarouselLoader presenter) => _presenter = presenter;
    }
}