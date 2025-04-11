namespace Domain
{
    public class Item
    {
        public Item(IItemView viewPrefab, IItemData itemData)
        {
            ViewPrefab = viewPrefab;
            ItemData = itemData;
        }

        public IItemView ViewPrefab { get; }
        public IItemData ItemData { get; }
    }
}