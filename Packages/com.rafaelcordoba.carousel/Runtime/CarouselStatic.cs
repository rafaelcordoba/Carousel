using System.Collections.Generic;
using System.Linq;
using Carousel.Runtime.Pooling;
using UnityEngine;

namespace Carousel.Runtime
{
    public class CarouselStatic : MonoBehaviour, ICarouselSelect
    {
        private readonly Dictionary<int, PoolObject> _poolObjects = new();
        private readonly HashSet<int> _visibleIndices = new();
        private IReadOnlyList<Item> _items = new List<Item>();
        private readonly List<AbstractItemView> _activeItemViews = new();
        private PrefabPooling _pooling;

        public Config Config { get; private set; }
        public int SelectedIndex { get; protected set; }
        public int DataCount => _items.Count;

        private void Awake() => 
            _pooling = new PrefabPooling();
        
        private void OnDestroy() => 
            _pooling.Dispose();

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

        public void Prev()
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

        protected void PositionItems(float centerIndex)
        {
            var (minVisibleIndex, maxVisibleIndex) = GetVisibleIndexRange(centerIndex);
            var visibleItemViews = new List<AbstractItemView>();

            foreach (var itemView in _activeItemViews)
            {
                var isVisible = UpdateItemVisibility(itemView, minVisibleIndex, maxVisibleIndex);
                if (!isVisible) 
                    continue;
                
                ApplyItemTransform(itemView, centerIndex);
                visibleItemViews.Add(itemView);
            }

            ReorderSiblings(Mathf.RoundToInt(centerIndex), visibleItemViews);
        }

        private static bool UpdateItemVisibility(AbstractItemView itemView, float minVisibleIndex, float maxVisibleIndex)
        {
            var isVisible = itemView.ItemIndex >= Mathf.Floor(minVisibleIndex) &&
                            itemView.ItemIndex <= Mathf.Ceil(maxVisibleIndex);
            itemView.gameObject.SetActive(isVisible);
            return isVisible;
        }
        
        private void ApplyItemTransform(AbstractItemView itemView, float centerIndex)
        {
            var itemGameObject = itemView.gameObject;
            var offsetFromCenter = itemView.ItemIndex - centerIndex;

            // Position
            var xPos = offsetFromCenter * Config.horizontalSpacing;
            var zPos = Mathf.Abs(offsetFromCenter) * Config.zOffsetStep;
            itemGameObject.transform.localPosition = new Vector3(xPos, 0f, zPos);

            // Rotation
            var yRot = -offsetFromCenter * Config.rotationYStep;
            itemGameObject.transform.localRotation = Quaternion.Euler(0f, yRot, 0f);

            // Scale
            var scaleFactor = 1.0f - Mathf.Clamp01(Mathf.Abs(offsetFromCenter) / Config.visibleItemsPerSide);
            var scale = Mathf.Lerp(Config.sideScale, 1.0f, scaleFactor);
            itemGameObject.transform.localScale = Vector3.one * scale;
        }

        private static void ReorderSiblings(int centerIndex, List<AbstractItemView> itemViews)
        {
            var sortedItems = itemViews
                .OrderBy(item => Mathf.Abs(item.ItemIndex - centerIndex))
                .ThenBy(item => item.ItemIndex)
                .ToList();

            for (var i = 0; i < sortedItems.Count; i++)
                sortedItems[i].transform.SetSiblingIndex(sortedItems.Count - 1 - i);
        }

        protected void UpdateVisibleViews(float? animatedCenterIndex = null)
        {
            var centerIndex = animatedCenterIndex ?? SelectedIndex;
            CalculateVisibleIndices(centerIndex);
            ReturnObsoleteViews();
            CreateAndActivateVisibleViews();
        }

        private void CalculateVisibleIndices(float centerIndex)
        {
            var (minVisibleIndex, maxVisibleIndex) = GetVisibleIndexRange(centerIndex);
            _visibleIndices.Clear();
            for (var i = Mathf.FloorToInt(minVisibleIndex); i <= Mathf.CeilToInt(maxVisibleIndex); i++)
                if (IsIndexValid(i)) 
                    _visibleIndices.Add(i);
        }

        private void ReturnObsoleteViews()
        {
            var indicesToRemove = _poolObjects.Keys.Except(_visibleIndices).ToList();
            foreach (var index in indicesToRemove)
            {
                if (!_poolObjects.TryGetValue(index, out var poolObject)) 
                    continue;
                
                _pooling.Return(poolObject);
                _poolObjects.Remove(index);
            }
        }

        private void CreateAndActivateVisibleViews()
        {
            _activeItemViews.Clear();
            foreach (var index in _visibleIndices)
            {
                var itemView = GetOrCreateView(index);
                if (itemView) 
                    _activeItemViews.Add(itemView);
            }
        }

        private AbstractItemView GetOrCreateView(int index)
        {
            if (_poolObjects.TryGetValue(index, out var po))
                return po.Instance.GetComponent<AbstractItemView>();

            var item = _items[index];
            var poolObject = _pooling.Get(item.ViewPrefab.gameObject);
            var itemView = poolObject.Instance.GetComponent<AbstractItemView>();
            itemView.transform.SetParent(transform);
            itemView.Initialize(index, item.ItemData, this);
            poolObject.Instance.SetActive(true); 
            _poolObjects[index] = poolObject;
            return itemView;
        }

        protected bool IsIndexValid(int index) 
            => index >= 0 && index < DataCount;

        private (float minVisibleIndex, float maxVisibleIndex) GetVisibleIndexRange(float centerIndex)
        {
            var config = Config;
            var minVisibleIndex = centerIndex - config.visibleItemsPerSide;
            var maxVisibleIndex = centerIndex + config.visibleItemsPerSide;
            return (minVisibleIndex, maxVisibleIndex);
        }
    }
}