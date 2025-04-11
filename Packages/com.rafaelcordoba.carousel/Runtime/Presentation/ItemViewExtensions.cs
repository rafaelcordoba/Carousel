using System.Collections.Generic;
using UnityEngine;

namespace Presentation
{
    public static class ItemViewExtensions
    {
        public static void ReorderSiblings(this List<AbstractItemView> itemViews, float centerIndex, int totalCount)
        {
            // Sort items in place by their absolute wrapped difference from centerIndex.
            itemViews.Sort((a, b) =>
            {
                var diffA = WrappedDiff(a.ItemIndex, centerIndex, totalCount);
                var diffB = WrappedDiff(b.ItemIndex, centerIndex, totalCount);
                return Mathf.Abs(diffA).CompareTo(Mathf.Abs(diffB));
            });

            // Set sibling indices such that items closer to the center are on top.
            for (var i = 0; i < itemViews.Count; i++)
                itemViews[i].transform.SetSiblingIndex(itemViews.Count - 1 - i);
        }
        
        private static float WrappedDiff(float itemIndex, float centerIndex, float totalCount)
        {
            var diff = itemIndex - centerIndex;
            var shiftedValue = diff + totalCount / 2f;
            return Mathf.Repeat(shiftedValue, totalCount) - totalCount / 2f;
        }
    }
}
