using UnityEngine;
using UnityEngine.EventSystems;

namespace Carousel.Runtime
{
    public abstract class AbstractItemView : MonoBehaviour, IPointerClickHandler
    {
        public int ItemIndex { get; private set; }
        private ICarouselSelect _carouselSelect;

        public void OnPointerClick(PointerEventData eventData) => 
            _carouselSelect?.Select(ItemIndex);

        public void Initialize(int index, IItemData itemData, ICarouselSelect select)
        {
            ItemIndex = index;
            _carouselSelect = select;
            OnInitialize(itemData);
        }

        protected abstract void OnInitialize(IItemData itemData);
    }
}