using UnityEngine;

namespace Carousel.Samples
{
    public static  class TransformExtensions
    {
        public static void DestroyChildren(this Transform transform)
        {
            foreach (object child in transform)
            {
                Object.Destroy(((Transform) child).gameObject);
            }
        }
    }
}