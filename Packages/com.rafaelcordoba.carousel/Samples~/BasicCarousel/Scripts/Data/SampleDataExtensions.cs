using System.Collections.Generic;
using Domain;

namespace Samples.Data
{
    public static class SampleDataExtensions
    {
        public static List<Item> ToCarouselItems(this SampleData sampleData)
        {
            var itemViewModels = new List<Item>();

            foreach (var data in sampleData.Datas)
            {
                var basicItemData = new BasicItemData(data.title, data.description);
                var prefab = data.customPrefab ? data.customPrefab : sampleData.DefaultPrefab;
                var carouselItem = new Item(prefab.gameObject, basicItemData);
                itemViewModels.Add(carouselItem);
            }

            foreach (var data in sampleData.DataIcons)
            {
                var iconItemData = new IconItemData(data.title, data.description, data.icon);
                var prefab = data.customPrefab ? data.customPrefab : sampleData.DefaultPrefab;
                var carouselItem = new Item(prefab.gameObject, iconItemData);
                itemViewModels.Add(carouselItem);
            }

            return itemViewModels;
        }
    }
}