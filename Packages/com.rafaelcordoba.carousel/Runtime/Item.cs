namespace Carousel.Runtime
{
    public class Item
    {
        public Item(AbstractItemView viewPrefab, IItemData itemData)
        {
            ViewPrefab = viewPrefab;
            ItemData = itemData;
        }

        public AbstractItemView ViewPrefab { get; }
        public IItemData ItemData { get; }
    }
}