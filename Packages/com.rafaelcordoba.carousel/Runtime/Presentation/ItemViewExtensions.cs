using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Presentation
{
    public static class ItemViewExtensions
    {
        public static void ReorderSiblings(this List<AbstractItemView> itemViews, int centerIndex)
        {
            var sortedItems = itemViews
                .OrderBy(item => Mathf.Abs(item.ItemIndex - centerIndex))
                .ThenBy(item => item.ItemIndex)
                .ToList();

            for (var i = 0; i < sortedItems.Count; i++)
                sortedItems[i].transform.SetSiblingIndex(sortedItems.Count - 1 - i);
        }
    }
}