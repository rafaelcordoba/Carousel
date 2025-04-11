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
                SelectedIndexChanged?.Invoke(previousIndex, _selectedIndex);
            }
        }

        public event Action<int, int> SelectedIndexChanged;

        public void First() => Select(0);
        public void Last() => Select(Items.Count - 1);
        public void Next() => Select((SelectedIndex + 1) % Items.Count);
        public void Prev() => Select((SelectedIndex - 1 + Items.Count) % Items.Count);
        public void Select(int index) => SelectedIndex = index;

        private bool IsIndexValid(int index)
            => index >= 0 && index < Items.Count;
    }
}