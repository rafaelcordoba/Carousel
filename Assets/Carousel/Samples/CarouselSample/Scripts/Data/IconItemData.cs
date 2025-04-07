using UnityEngine;

namespace Sample.Data
{
    public class IconItemData : BasicItemData
    {
        public IconItemData(string title, string description, Sprite icon) : base(title, description)
        {
            Icon = icon;
        }

        public Sprite Icon { get; set; }
    }
}