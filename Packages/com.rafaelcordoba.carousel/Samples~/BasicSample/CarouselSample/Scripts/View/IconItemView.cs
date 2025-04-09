using Carousel.Runtime;
using Sample.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
    public class IconItemView : SimpleItemView
    {
        [SerializeField] private Image icon;

        protected override void OnInitialize(IItemData itemData)
        {
            base.OnInitialize(itemData);
            var iconItemData = (IconItemData)itemData;
            icon.sprite = iconItemData.Icon;
        }
    }
}