using Carousel.Runtime;

namespace Carousel.Samples.Data
{
    public class BasicItemData : IItemData
    {
        public BasicItemData(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public string Title { get; set; }
        public string Description { get; set; }
    }
}