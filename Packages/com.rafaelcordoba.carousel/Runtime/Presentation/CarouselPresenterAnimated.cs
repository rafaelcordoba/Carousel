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
            var elapsedTime = Time.time - _animationStartTime;
            _animationProgress = Mathf.Clamp01(elapsedTime / Model.Config.transitionDuration);

            var itemCount = Model.Items.Count;
            float difference = _targetSelectedIndex - _animationStartIndex;
    
            if (difference > itemCount / 2f)
                difference -= itemCount;
            else if (difference < -itemCount / 2f)
                difference += itemCount;
    
            var virtualTarget = _animationStartIndex + difference;
    
            var currentCenterIndex = Mathf.Lerp(_animationStartIndex, virtualTarget, _animationProgress);
            RedrawAt(currentCenterIndex);

            if (_animationProgress >= 1f)
            {
                Model.SelectedIndex = _targetSelectedIndex;
                _isAnimating = false;
            }
        }
    }
}