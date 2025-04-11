using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Carousel.Runtime
{
    [RequireComponent(typeof(PlayerInput))]
    public class SwipeAndDragDetector : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float minSwipeDistance = 50f;
        [SerializeField] private float minDragDistance = 50f;
        [SerializeField] private float maxSwipeTime = 0.5f;
    
        [Header("Events")]
        private UnityEvent<Vector2> _onSwipe;
        private UnityEvent<Vector2> _onDragging;
    
        private Vector2 _startPosition;
        private Vector2 _currentPosition;
        private float _startTime;
        private bool _isDragging;
        private bool _hasReachedMinDragDistance;
    
        private InputAction _touchPositionAction;
        private InputAction _touchPressAction;
        
        public UnityEvent<Vector2> OnSwipe => _onSwipe ??= new UnityEvent<Vector2>();
        public UnityEvent<Vector2> OnDragging => _onDragging ??= new UnityEvent<Vector2>();
    
        private void Awake()
        {
            var playerInput = GetComponent<PlayerInput>();
        
            _touchPositionAction = playerInput.actions["TouchPosition"];
            _touchPressAction = playerInput.actions["TouchPress"];
        
            if (!EnhancedTouchSupport.enabled)
            {
                EnhancedTouchSupport.Enable();
            }
        }
    
        private void OnEnable()
        {
            _touchPressAction.started += OnTouchStarted;
            _touchPressAction.canceled += OnTouchEnded;
        }
    
        private void OnDisable()
        {
            _touchPressAction.started -= OnTouchStarted;
            _touchPressAction.canceled -= OnTouchEnded;
        }
    
        private void Update()
        {
            if (_isDragging)
            {
                _currentPosition = _touchPositionAction.ReadValue<Vector2>();
                Vector2 dragDelta = _currentPosition - _startPosition;
                float dragDistance = dragDelta.magnitude;
                
                // Check if we've reached the minimum drag distance
                if (dragDistance >= minDragDistance)
                {
                    _hasReachedMinDragDistance = true;
                    _onDragging?.Invoke(dragDelta);
                    // Debug.Log("Dragging " + dragDelta);
                }
            }
        }
    
        private void OnTouchStarted(InputAction.CallbackContext context)
        {
            _startPosition = _touchPositionAction.ReadValue<Vector2>();
            _currentPosition = _startPosition;
            _startTime = Time.time;
            _isDragging = true;
            _hasReachedMinDragDistance = false;
        }
    
        private void OnTouchEnded(InputAction.CallbackContext context)
        {
            _isDragging = false;
        
            var endPosition = _touchPositionAction.ReadValue<Vector2>();
        
            var swipeDirection = endPosition - _startPosition;
            var swipeDistance = swipeDirection.magnitude;
            var swipeTime = Time.time - _startTime;
        
            if (swipeDistance >= minSwipeDistance && swipeTime <= maxSwipeTime)
            {
                _onSwipe?.Invoke(swipeDirection);
                // Debug.Log($"Swipe {swipeDirection}");
            }
        }
    }
}