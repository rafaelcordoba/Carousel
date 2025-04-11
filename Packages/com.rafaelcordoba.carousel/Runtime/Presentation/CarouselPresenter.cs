using System.Collections.Generic;
using System.Linq;
using Domain;
using Pooling;
using UnityEngine;

namespace Presentation
{
    public class CarouselPresenter : MonoBehaviour
    {
        private readonly Dictionary<int, PoolObject> _poolObjects = new();
        private readonly HashSet<int> _visibleIndices = new();
        private readonly List<AbstractItemView> _activeItemViews = new();
        private PrefabPooling _pooling;
        public Carousel Model;

        private void Awake() => 
            _pooling = new PrefabPooling();

        private void OnDestroy()
        {
            _pooling.Dispose();
            if (Model == null) 
                return;
            Model.SelectedIndexChanged -= OnSelectedIndexChanged;
        }

        public void Initialize(Carousel carousel)
        {
            Model = carousel;
            Model.SelectedIndexChanged += OnSelectedIndexChanged;
            RedrawAt(Model.SelectedIndex);
        }

        protected virtual void OnSelectedIndexChanged(int fromIndex, int toIndex) => 
            RedrawAt(toIndex);

        public void RedrawAt(float centerIndex)
        {
            CalculateVisibleIndices(centerIndex);
            ReturnObsoleteViews();
            CreateAndActivateVisibleViews();
            PositionItems(centerIndex);
        }

        private void CalculateVisibleIndices(float centerIndex)
        {
            var (minVisibleIndex, maxVisibleIndex) = GetVisibleIndexRange(centerIndex);
            _visibleIndices.Clear();
            for (var i = Mathf.FloorToInt(minVisibleIndex); i <= Mathf.CeilToInt(maxVisibleIndex); i++)
            {
                if (Model.Items.Count > 0)
                    _visibleIndices.Add(GetWrappedIndex(i));
            }
        }

        private int GetWrappedIndex(int index)
        {
            var count = Model.Items.Count;
            return (index % count + count) % count;
        }

        private (float minVisibleIndex, float maxVisibleIndex) GetVisibleIndexRange(float centerIndex)
        {
            var config = Model.Config;
            var minVisibleIndex = centerIndex - config.visibleItemsPerSide;
            var maxVisibleIndex = centerIndex + config.visibleItemsPerSide;
            return (minVisibleIndex, maxVisibleIndex);
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

            var item = Model.Items[index];
            var poolObject = _pooling.Get(item.ViewPrefab);
            _poolObjects[index] = poolObject;
            
            var itemView = poolObject.Instance.GetComponent<AbstractItemView>();
            itemView.transform.SetParent(transform);
            itemView.transform.localPosition = Vector3.zero;
            itemView.transform.localRotation = Quaternion.identity;
            itemView.transform.localScale = Vector3.one;
            itemView.Initialize(index, item.ItemData, Model);
            poolObject.Instance.SetActive(true); 
            return itemView;
        }

        private void PositionItems(float centerIndex)
        {
            var itemViewsToSort = new List<AbstractItemView>();
            var itemCount = Model.Items.Count;
            if (itemCount == 0) return;

            foreach (var itemView in _activeItemViews)
            {
                var itemIndex = itemView.ItemIndex;
                var virtualIndex = itemIndex + Mathf.Round((centerIndex - itemIndex) / itemCount) * itemCount;

                itemView.ApplyItemTransform(Model.Config, centerIndex, virtualIndex);
                itemViewsToSort.Add(itemView);
            }

            itemViewsToSort.ReorderSiblings(Mathf.RoundToInt(centerIndex), itemCount);
        }
    }
}