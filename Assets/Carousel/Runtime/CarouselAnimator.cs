using System.Collections.Generic;
using UnityEngine;

namespace Carousel.Runtime
{
    public class CarouselAnimator : MonoBehaviour
    {
        private float _animationProgress = 1f;
        private int _animationStartIndex;
        private float _animationStartTime;
        private Carousel3D _carousel;
        private bool _isAnimating;
        private int _targetSelectedIndex;

        private void Update()
        {
            if (_isAnimating)
                UpdateAnimation();
        }

        public void Initialize(Carousel3D carousel)
        {
            _carousel = carousel;
            PositionItems();
        }

        public void StartAnimation(int targetIndex)
        {
            if (_isAnimating)
                return;

            // Start a new animation
            _animationStartIndex = _carousel.SelectedIndex;
            _targetSelectedIndex = targetIndex;
            _animationStartTime = Time.time;
            _animationProgress = 0f;
            _isAnimating = true;
        }

        private void PositionItems(float? animatedCenterIndex = null)
        {
            var config = _carousel.Config;
            var itemViews = _carousel.ItemViews;
            var centerIndex = animatedCenterIndex ?? _carousel.SelectedIndex;
            var minVisibleIndex = centerIndex - config.visibleItemsPerSide;
            var maxVisibleIndex = centerIndex + config.visibleItemsPerSide;

            var visibleItemViews = new List<AbstractItemView>();

            foreach (var itemView in itemViews)
            {
                var item = itemView.gameObject;
                if (!item)
                    continue;

                var offsetFromCenter = itemView.ItemIndex - centerIndex;
                var isVisible = itemView.ItemIndex >= Mathf.Floor(minVisibleIndex) &&
                                itemView.ItemIndex <= Mathf.Ceil(maxVisibleIndex);
                item.SetActive(isVisible);

                if (!isVisible)
                    continue;

                // Position
                var xPos = offsetFromCenter * config.horizontalSpacing;
                var zPos = Mathf.Abs(offsetFromCenter) * config.zOffsetStep;
                item.transform.localPosition = new Vector3(xPos, 0f, zPos);

                // Rotation
                var yRot = -offsetFromCenter * config.rotationYStep;
                item.transform.localRotation = Quaternion.Euler(0f, yRot, 0f);

                // Scale
                var scaleFactor = animatedCenterIndex.HasValue
                    ? Mathf.Clamp01(1.0f - Mathf.Abs(offsetFromCenter))
                    : offsetFromCenter == 0
                        ? 1f
                        : 0f;
                var scale = Mathf.Lerp(config.sideScale, 1.0f, scaleFactor);
                item.transform.localScale = Vector3.one * scale;

                // Add to list for reordering
                visibleItemViews.Add(itemView);
            }

            CarouselSiblings.Reorder(Mathf.RoundToInt(centerIndex), visibleItemViews);
        }

        private void UpdateAnimation()
        {
            var elapsedTime = Time.time - _animationStartTime;
            _animationProgress = Mathf.Clamp01(elapsedTime / _carousel.Config.transitionDuration);

            var currentCenterIndex = Mathf.Lerp(_animationStartIndex, _targetSelectedIndex, _animationProgress);

            // Update visible views during animation
            _carousel.UpdateVisibleViews(currentCenterIndex);
            PositionItems(currentCenterIndex);

            if (_animationProgress >= 1f)
            {
                _isAnimating = false;
                _carousel.SelectedIndex = _targetSelectedIndex;
            }
        }
    }
}