using Carousel.Runtime;
using Carousel.Samples.Data;
using UnityEngine;

namespace Carousel.Samples
{
    public class SampleCarousels : MonoBehaviour
    {
        [SerializeField] private Transform container1;
        [SerializeField] private Transform container2;

        [SerializeField] private CarouselConfig config1;
        [SerializeField] private CarouselConfig config2;

        [SerializeField] private SampleData sampleData1;
        [SerializeField] private SampleData sampleData2;

        private void Start()
        {
            var carousel1Obj = new GameObject("Carousel 1");
            var carousel1 = carousel1Obj.AddComponent<Carousel3D>();
            var items = sampleData1.ToCarouselItems();
            var selectedIndex = items.Count / 2;
            carousel1.Initialize(config1, items, selectedIndex);
            carousel1Obj.AddComponent<SwipeInputHandler>().Initialize(carousel1);
            carousel1Obj.transform.SetParent(container1, false);

            var carousel2Obj = new GameObject("Carousel 2");
            var carousel2 = carousel2Obj.AddComponent<Carousel3D>();
            var items2 = sampleData2.ToCarouselItems();
            carousel2.Initialize(config2, items2, 0);
            carousel2Obj.AddComponent<SwipeInputHandler>().Initialize(carousel2);
            carousel2Obj.transform.SetParent(container2, false);
        }
    }
}