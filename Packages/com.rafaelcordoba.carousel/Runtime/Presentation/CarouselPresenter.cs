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
            Model.SelectedIndexBeforeChange -= OnSelectedIndexBeforeChanged;
            Model.SelectedIndexChanged -= OnSelectedIndexChanged;
        }

        public void Initialize(Carousel carousel)
        {
            Model = carousel;
            Model.SelectedIndexBeforeChange += OnSelectedIndexBeforeChanged;
            Model.SelectedIndexChanged += OnSelectedIndexChanged;
            RedrawAt(Model.SelectedIndex);
        }
        
        protected virtual void OnSelectedIndexBeforeChanged(int before, int after) {}

        private void OnSelectedIndexChanged(int index) => 
            RedrawAt(index);

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
                if (Model.IsIndexValid(i)) 
                    _visibleIndices.Add(i);
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
            var itemView = poolObject.Instance.GetComponent<AbstractItemView>();
            itemView.transform.SetParent(transform);
            itemView.transform.localPosition = Vector3.zero;
            itemView.transform.localRotation = Quaternion.identity;
            itemView.transform.localScale = Vector3.one;
            itemView.Initialize(index, item.ItemData, Model);
            poolObject.Instance.SetActive(true); 
            _poolObjects[index] = poolObject;
            return itemView;
        }

        private void PositionItems(float centerIndex)
        {
            var (minVisibleIndex, maxVisibleIndex) = GetVisibleIndexRange(centerIndex);
            var itemViews = new List<AbstractItemView>();

            foreach (var itemView in _activeItemViews)
            {
                var isVisible = itemView.ItemIndex >= Mathf.Floor(minVisibleIndex) &&
                                 itemView.ItemIndex <= Mathf.Ceil(maxVisibleIndex);
                itemView.gameObject.SetActive(isVisible);
                
                if (!isVisible) 
                    continue;
                
                itemView.ApplyItemTransform(Model.Config, centerIndex);
                itemViews.Add(itemView);
            }

            itemViews.ReorderSiblings(Mathf.RoundToInt(centerIndex));
        }
    }
}