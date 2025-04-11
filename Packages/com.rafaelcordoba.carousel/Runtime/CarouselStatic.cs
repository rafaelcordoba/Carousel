using System.Collections.Generic;
using System.Linq;
using Carousel.Runtime.Pooling;
using UnityEngine;

namespace Carousel.Runtime
{
    public class CarouselStatic : MonoBehaviour, ICarouselInputHandler
    {
        private readonly Dictionary<int, AbstractItemView> _activeViews = new();
        private readonly HashSet<int> _visibleIndices = new();
        private IReadOnlyList<Item> _items = new List<Item>();
        private PrefabPooling _pooling;

        public List<AbstractItemView> ItemViews { get; } = new();
        public Config Config { get; private set; }
        public int SelectedIndex { get; protected set; }
        public int DataCount => _items.Count;

        private void Awake() => _pooling = new PrefabPooling();
        private void OnDestroy() => _pooling.Dispose();

        public void Initialize(Config config, IReadOnlyList<Item> items, int selectedIndex)
        {
            Config = config;
            _items = items;
            SelectedIndex = selectedIndex;
            UpdateVisibleViews();
            PositionItems(selectedIndex);
        }

        public void Next()
        {
            if (SelectedIndex < DataCount - 1)
                Select(SelectedIndex + 1);
        }

        public void Previous()
        {
            if (SelectedIndex > 0)
                Select(SelectedIndex - 1);
        }

        public void First()
        {
            if (SelectedIndex > 0)
                Select(0);
        }

        public void Last()
        {
            if (SelectedIndex < DataCount - 1)
                Select(DataCount - 1);
        }

        public virtual void Select(int index)
        {
            if (IsIndexValid(index) && index != SelectedIndex)
            {
                SelectedIndex = index;
                UpdateVisibleViews();
                PositionItems(index);
            }
            else if (!IsIndexValid(index))
                Debug.LogError($"Invalid index {index}. Must be between 0 and {DataCount - 1}");
        }

        public void Refresh()
        {
            PositionItems(SelectedIndex);
            UpdateVisibleViews();
        }

        public void UpdateVisibleViews(float? animatedCenterIndex = null)
        {
            var centerIndex = animatedCenterIndex ?? SelectedIndex;
            var (minVisibleIndex, maxVisibleIndex) = CalculateVisibleIndexRange(centerIndex);

            // Calculate which indices should be visible
            _visibleIndices.Clear();
            for (var i = Mathf.FloorToInt(minVisibleIndex); i <= Mathf.CeilToInt(maxVisibleIndex); i++)
                if (i >= 0 && i < DataCount)
                    _visibleIndices.Add(i);

            // Remove views that are no longer visible
            var indicesToRemove = new List<int>();
            foreach (var kvp in _activeViews)
                if (!_visibleIndices.Contains(kvp.Key))
                    indicesToRemove.Add(kvp.Key);

            foreach (var index in indicesToRemove)
            {
                var view = _activeViews[index];
                _pooling.Return(new PoolObject(view.ViewPrefab, view.gameObject));
                _activeViews.Remove(index);
            }

            // Create or update views for visible indices
            ItemViews.Clear();
            foreach (var index in _visibleIndices)
            {
                if (!_activeViews.TryGetValue(index, out var itemView))
                {
                    var item = _items[index];
                    var poolObject = _pooling.Get(item.ViewPrefab.gameObject);
                    itemView = poolObject.Instance.GetComponent<AbstractItemView>();
                    itemView.ViewPrefab = item.ViewPrefab.gameObject;
                    itemView.transform.SetParent(transform);
                    itemView.Initialize(index, item.ItemData, this);
                    _activeViews[index] = itemView;
                }

                ItemViews.Add(itemView);
            }
        }

        protected void PositionItems(float centerIndex)
        {
            var itemViews = ItemViews;
            var (minVisibleIndex, maxVisibleIndex) = CalculateVisibleIndexRange(centerIndex);

            var visibleItemViews = new List<AbstractItemView>();

            foreach (var itemView in itemViews)
            {
                var item = itemView.gameObject;
                if (!item)
                    continue;

                var offsetFromCenter = itemView.ItemIndex - centerIndex;
                var isVisible = itemView.ItemIndex >= Mathf.Floor(minVisibleIndex) &&
                                itemView.ItemIndex <= Mathf.Ceil(maxVisibleIndex);
                item.SetActive(isVisible);

                if (!isVisible)
                    continue;

                // Position
                var xPos = offsetFromCenter * Config.horizontalSpacing;
                var zPos = Mathf.Abs(offsetFromCenter) * Config.zOffsetStep;
                item.transform.localPosition = new Vector3(xPos, 0f, zPos);

                // Rotation
                var yRot = -offsetFromCenter * Config.rotationYStep;
                item.transform.localRotation = Quaternion.Euler(0f, yRot, 0f);

                // Scale
                var scaleFactor = Mathf.Clamp01(1.0f - Mathf.Abs(offsetFromCenter));
                var scale = Mathf.Lerp(Config.sideScale, 1.0f, scaleFactor);
                item.transform.localScale = Vector3.one * scale;

                // Add to list for reordering
                visibleItemViews.Add(itemView);
            }

            ReorderSiblings(Mathf.RoundToInt(centerIndex), visibleItemViews);
        }

        protected bool IsIndexValid(int index) => index >= 0 && index < DataCount;

        private (float minVisibleIndex, float maxVisibleIndex) CalculateVisibleIndexRange(float centerIndex)
        {
            var config = Config;
            var minVisibleIndex = centerIndex - config.visibleItemsPerSide;
            var maxVisibleIndex = centerIndex + config.visibleItemsPerSide;
            return (minVisibleIndex, maxVisibleIndex);
        }

        private static void ReorderSiblings(int centerIndex, List<AbstractItemView> itemViews)
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