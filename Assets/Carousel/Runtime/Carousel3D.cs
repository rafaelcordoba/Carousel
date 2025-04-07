using System.Collections.Generic;
using Carousel.Runtime.Pooling;
using UnityEngine;

namespace Carousel.Runtime
{
    public class Carousel3D : MonoBehaviour, ICarouselInputHandler
    {
        public readonly List<AbstractItemView> ItemViews = new();
        private readonly Dictionary<int, AbstractItemView> _activeViews = new();
        private CarouselAnimator _animator;
        private PrefabPooling _pooling;

        private IReadOnlyList<CarouselItem> _items = new List<CarouselItem>();
        private readonly HashSet<int> _visibleIndices = new();
        public CarouselConfig Config { get; private set; }
        public int SelectedIndex { get; set; }
        public int DataCount => _items.Count;

        private void Awake()
        {
            _animator = gameObject.AddComponent<CarouselAnimator>();
            _pooling = new PrefabPooling();
        }

        public void Initialize(CarouselConfig config, IReadOnlyList<CarouselItem> items, int selectedIndex)
        {
            Config = config;
            _items = items;
            SelectedIndex = selectedIndex;
            UpdateVisibleViews();
            InitializeAnimator();
        }

        public void InitializeAnimator()
        {
            _animator.Initialize(this);
        }

        public void UpdateVisibleViews(float? animatedCenterIndex = null)
        {
            var config = Config;
            var centerIndex = animatedCenterIndex ?? SelectedIndex;
            var minVisibleIndex = centerIndex - config.visibleItemsPerSide;
            var maxVisibleIndex = centerIndex + config.visibleItemsPerSide;

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

        public void Select(int index)
        {
            if (index >= 0 && index < DataCount && index != SelectedIndex)
                _animator.StartAnimation(index);
            else if (index < 0 || index >= DataCount)
                Debug.LogError($"Invalid index {index}. Must be between 0 and {DataCount - 1}");
        }
    }
}