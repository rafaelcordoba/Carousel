namespace Carousel.Runtime
{
    public class CarouselItem
    {
        public CarouselItem(AbstractItemView viewPrefab, IItemData itemData)
        {
            ViewPrefab = viewPrefab;
            ItemData = itemData;
        }

        public AbstractItemView ViewPrefab { get; set; }
        public IItemData ItemData { get; set; }
    }
}