using Domain;
using Samples.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.View
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