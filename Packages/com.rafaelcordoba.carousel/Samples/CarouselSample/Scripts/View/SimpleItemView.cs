using Carousel.Runtime;
using Sample.Data;
using TMPro;
using UnityEngine;

namespace Sample
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