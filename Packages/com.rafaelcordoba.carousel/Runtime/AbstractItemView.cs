using UnityEngine;
using UnityEngine.EventSystems;

namespace Carousel.Runtime
{
    public abstract class AbstractItemView : MonoBehaviour, IPointerClickHandler
    {
        public int ItemIndex { get; private set; }
        public GameObject ViewPrefab { get; set; }
        private ICarouselInputHandler _carouselInputHandler;

        public void OnPointerClick(PointerEventData eventData) => 
            _carouselInputHandler?.Select(ItemIndex);

        public void Initialize(int index, IItemData itemData, ICarouselInputHandler inputHandler)
        {
            ItemIndex = index;
            _carouselInputHandler = inputHandler;
            OnInitialize(itemData);
        }

        protected abstract void OnInitialize(IItemData itemData);
    }
}