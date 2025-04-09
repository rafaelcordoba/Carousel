namespace Carousel.Runtime
{
    public interface ICarouselInputHandler
    {
        void Select(int index);
        void Next();
        void Previous();
    }
} 