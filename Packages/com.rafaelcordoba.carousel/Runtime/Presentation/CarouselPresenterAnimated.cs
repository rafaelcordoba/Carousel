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

        protected override void OnSelectedIndexBeforeChanged(int fromIndex, int toIndex) => 
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
            var currentCenterIndex = Mathf.Lerp(_animationStartIndex, _targetSelectedIndex, _animationProgress);

            RedrawAt(currentCenterIndex);

            var animationFinished = _animationProgress >= 1f;
            if (!animationFinished) 
                return;
            
            Model.SelectedIndex = _targetSelectedIndex;
            _isAnimating = false;
        }
    }
}