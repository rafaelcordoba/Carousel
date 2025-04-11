using UnityEngine;

namespace Presentation
{
    public class CarouselPresenterAnimated : CarouselPresenter
    {
        private float _animationProgress;
        private int _animationStartIndex;
        private float _animationStartTime;
        private bool _isAnimating;
        private int _targetSelectedIndex;

        protected override void OnSelectedIndexChanged(int fromIndex, int toIndex) => 
            StartAnimation(fromIndex, toIndex);

        private void StartAnimation(int fromIndex, int toIndex)
        {
            if (_isAnimating)
                return;

            _animationStartIndex = fromIndex;
            _targetSelectedIndex = toIndex;
            _animationStartTime = Time.time;
            _animationProgress = 0f;
            _isAnimating = true;
        }

        private void Update()
        {
            if (_isAnimating)
                UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            // Calculate elapsed time and progress
            var elapsedTime = Time.time - _animationStartTime;
            _animationProgress = Mathf.Clamp01(elapsedTime / Model.Config.transitionDuration);

            // Compute the shortest path for the animation.
            int itemCount = Model.Items.Count;
            float difference = _targetSelectedIndex - _animationStartIndex;
    
            // Adjust difference for infinite looping (shortest distance).
            if (difference > itemCount / 2f)
                difference -= itemCount;
            else if (difference < -itemCount / 2f)
                difference += itemCount;
    
            // Virtual target represents the "real" target on the carousel that takes into account looping.
            float virtualTarget = _animationStartIndex + difference;
    
            // Interpolate from the current index to the virtual target.
            var currentCenterIndex = Mathf.Lerp(_animationStartIndex, virtualTarget, _animationProgress);
            RedrawAt(currentCenterIndex);

            // If animation finished, update the actual index.
            if (_animationProgress >= 1f)
            {
                Model.SelectedIndex = _targetSelectedIndex; // This wraps to the correct item.
                _isAnimating = false;
            }
        }
    }
}