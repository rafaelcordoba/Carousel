using Carousel.Samples.Data;
using Domain;
using Presentation;
using TMPro;
using UnityEngine;

namespace Carousel.Samples.View
{
    public class SimpleItemView : AbstractItemView
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;

        protected override void OnInitialize(IItemData itemData)
        {
            var basicItemData = (BasicItemData)itemData;
            title.text = basicItemData.Title;
            description.text = basicItemData.Description;
        }
    }
}