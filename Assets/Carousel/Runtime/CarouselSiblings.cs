using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Carousel.Runtime
{
    public static class CarouselSiblings
    {
        public static void Reorder(int centerIndex, List<AbstractItemView> itemViews)
        {
            // Sort siblings by their distance from center, with the center item first
            var sortedItems = itemViews
                .OrderBy(item => Mathf.Abs(item.ItemIndex - centerIndex))
                .ThenBy(item => item.ItemIndex)
                .ToList();

            // Set sibling indices in reverse order so that center item is at the top
            for (var i = 0; i < sortedItems.Count; i++)
                // Reverse the index so that the center item (first in sortedItems) gets the highest index
                sortedItems[i].transform.SetSiblingIndex(sortedItems.Count - 1 - i);
        }
    }
}