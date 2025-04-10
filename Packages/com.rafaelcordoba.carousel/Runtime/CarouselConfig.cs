using UnityEngine;

namespace Carousel.Runtime
{
    [CreateAssetMenu(fileName = "CarouselConfig", menuName = "Carousel/New Configuration")]
    public class CarouselConfig : ScriptableObject
    {
        [Tooltip("How many items in the carousel.")] [Range(1, 2)]
        public int visibleItemsPerSide = 2;

        [Tooltip("Horizontal spacing between items.")]
        public float horizontalSpacing = 212;

        [Tooltip("Rotation offset (in degrees) around Y-axis per step.")]
        public float rotationYStep = 10f;

        [Tooltip("Additional Z offset for side items.")]
        public float zOffsetStep = 85f;

        [Tooltip("Scale factor for side items vs. center item (1 = same size).")]
        public float sideScale = 1f;

        [Tooltip("Duration of the transition animation in seconds.")]
        public float transitionDuration = 0.25f;
    }
}