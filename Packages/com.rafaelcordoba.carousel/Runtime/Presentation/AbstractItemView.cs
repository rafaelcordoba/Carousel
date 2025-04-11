using Domain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Presentation
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

        public GameObject GameObject => gameObject;

        protected abstract void OnInitialize(IItemData itemData);
        
        public void ApplyItemTransform(Config config, float centerIndex, float virtualIndex)
        {
            var offsetFromCenter = virtualIndex - centerIndex;

            // Position
            var xPos = offsetFromCenter * config.horizontalSpacing;
            var zPos = Mathf.Abs(offsetFromCenter) * config.zOffsetStep;
            transform.localPosition = new Vector3(xPos, 0f, zPos);

            // Rotation
            var yRot = -offsetFromCenter * config.rotationYStep;
            transform.localRotation = Quaternion.Euler(0f, yRot, 0f);

            // Scale
            var scaleFactor = 1.0f - Mathf.Clamp01(Mathf.Abs(offsetFromCenter) / config.visibleItemsPerSide);
            var scale = Mathf.Lerp(config.sideScale, 1.0f, scaleFactor);
            transform.localScale = Vector3.one * scale;
        }
    }
}