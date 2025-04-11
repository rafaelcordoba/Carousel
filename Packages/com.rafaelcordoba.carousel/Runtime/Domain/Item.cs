using UnityEngine;

namespace Domain
{
    public class Item
    {
        public Item(GameObject viewPrefab, IItemData itemData)
        {
            ViewPrefab = viewPrefab;
            ItemData = itemData;
        }

        public GameObject ViewPrefab { get; }
        public IItemData ItemData { get; }
    }
}