using System;
using System.Collections.Generic;

namespace Domain
{
    public class Carousel : ICarouselSelect
    {
        public Carousel(Config config, IReadOnlyList<Item> items, int selectedIndex)
        {
            Config = config;
            Items = items;
            SelectedIndex = selectedIndex;
        }

        public Config Config { get; }
        public IReadOnlyList<Item> Items { get; }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (!IsIndexValid(value))
                    throw new ArgumentOutOfRangeException(nameof(value), "Index is out of range.");
                
                var previousIndex = _selectedIndex;
                _selectedIndex = value;
                OnSelectedIndexChanged(previousIndex, _selectedIndex);
            }
        }

        public event Action<int, int> SelectedIndexChanged;

        protected virtual void OnSelectedIndexChanged(int fromIndex, int toIndex)
        {
            SelectedIndexChanged?.Invoke(fromIndex, toIndex);
        }

        public void First()
        {
            Select(0);
        }

        public void Last()
        {
            Select(Items.Count - 1);
        }

        public void Next()
        {
            int nextIndex = (SelectedIndex + 1) % Items.Count;
            Select(nextIndex);
        }

        public void Prev()
        {
            int prevIndex = (SelectedIndex - 1 + Items.Count) % Items.Count;
            Select(prevIndex);
        }

        public void Select(int index)
        {
            SelectedIndex = index;
        }

        public bool IsIndexValid(int index)
            => index >= 0 && index < Items.Count;
    }
}