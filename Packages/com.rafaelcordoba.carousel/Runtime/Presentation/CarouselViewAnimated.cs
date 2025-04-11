using UnityEngine;

namespace Presentation
{
    public class CarouselViewAnimated : CarouselViewStatic
    {
        private float _animationProgress = 1f;
        private int _animationStartIndex;
        private float _animationStartTime;
        private bool _isAnimating;
        private int _targetSelectedIndex;

        private void Update()
        {
            if (_isAnimating)
                UpdateAnimation();
        }

        public override void Select(int index)
        {
            if (IsIndexValid(index) && index != SelectedIndex)
                StartAnimation(index);
            else if (!IsIndexValid(index))
                Debug.LogError($"Invalid index {index}. Must be between 0 and {DataCount - 1}");
        }

        private void StartAnimation(int targetIndex)
        {
            if (_isAnimating)
                return;

            _animationStartIndex = SelectedIndex;
            _targetSelectedIndex = targetIndex;
            _animationStartTime = Time.time;
            _animationProgress = 0f;
            _isAnimating = true;
        }

        private void UpdateAnimation()
        {
            var elapsedTime = Time.time - _animationStartTime;
            _animationProgress = Mathf.Clamp01(elapsedTime / Config.transitionDuration);
            var currentCenterIndex = Mathf.Lerp(_animationStartIndex, _targetSelectedIndex, _animationProgress);

            UpdateVisibleViews(currentCenterIndex);
            PositionItems(currentCenterIndex);

            var animationFinished = _animationProgress >= 1f;
            if (!animationFinished) 
                return;
            
            _isAnimating = false;
            SelectedIndex = _targetSelectedIndex;
        }
    }
}