using System.Collections.Generic;
using Carousel.Runtime;

namespace Carousel.Samples.Data
{
    public static class SampleDataExtensions
    {
        public static List<CarouselItem> ToCarouselItems(this SampleData sampleData)
        {
            var itemViewModels = new List<CarouselItem>();

            foreach (var data in sampleData.Datas)
            {
                var basicItemData = new BasicItemData(data.title, data.description);
                var itemView = data.customPrefab ? data.customPrefab : sampleData.DefaultPrefab;
                var carouselItem = new CarouselItem(itemView, basicItemData);
                itemViewModels.Add(carouselItem);
            }

            foreach (var data in sampleData.DataIcons)
            {
                var iconItemData = new IconItemData(data.title, data.description, data.icon);
                var itemView = data.customPrefab ? data.customPrefab : sampleData.DefaultPrefab;
                var carouselItem = new CarouselItem(itemView, iconItemData);
                itemViewModels.Add(carouselItem);
            }

            return itemViewModels;
        }
    }
}