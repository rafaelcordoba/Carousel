using UnityEngine;

namespace Samples
{
    public static  class TransformExtensions
    {
        public static void DestroyChildren(this Transform transform)
        {
            foreach (var child in transform)
            {
                Object.Destroy(((Transform) child).gameObject);
            }
        }
    }
}