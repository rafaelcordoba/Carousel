using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Inputs
{
    [RequireComponent(typeof(PlayerInput))]
    public class SwipeAndDragDetector : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float minSwipeDistance = 50f;
        [SerializeField] private float minDragDistance = 50f;
        [SerializeField] private float maxSwipeTime = 0.5f;
        [SerializeField] private float maxDragInterval = 1.0f;
    
        [Header("Events")]
        private UnityEvent<Vector2> _onSwipe;
        private UnityEvent<Vector2> _onDragging;
    
        private Vector2 _startPosition;
        private Vector2 _currentPosition;
        private float _startTime;
        private bool _isDragging;
        private float _lastDragTime;
    
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
            if (!_isDragging) 
                return;
            
            _currentPosition = _touchPositionAction.ReadValue<Vector2>();
            var dragDelta = _currentPosition - _startPosition;
            var dragDistance = dragDelta.magnitude;

            if (!(dragDistance >= minDragDistance)) 
                return;
                
            var currentTime = Time.time;
            if (currentTime - _lastDragTime >= maxDragInterval)
            {
                _onDragging?.Invoke(dragDelta);
                _lastDragTime = currentTime;
            }
        }
    
        private void OnTouchStarted(InputAction.CallbackContext context)
        {
            _startPosition = _touchPositionAction.ReadValue<Vector2>();
            _currentPosition = _startPosition;
            _startTime = Time.time;
            _lastDragTime = _startTime;
            _isDragging = true;
        }
    
        private void OnTouchEnded(InputAction.CallbackContext context)
        {
            _isDragging = false;
        
            var endPosition = _touchPositionAction.ReadValue<Vector2>();
        
            var swipeDirection = endPosition - _startPosition;
            var swipeDistance = swipeDirection.magnitude;
            var swipeTime = Time.time - _startTime;
        
            if (swipeDistance >= minSwipeDistance && swipeTime <= maxSwipeTime) 
                _onSwipe?.Invoke(swipeDirection);
        }
    }
}