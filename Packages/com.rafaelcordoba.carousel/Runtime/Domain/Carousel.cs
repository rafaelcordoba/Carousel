using System;
using System.Collections.Generic;

namespace Domain
{
    public class Carousel : ICarouselSelect
    {
        private int _selectedIndex;

        public Carousel(Config config, IReadOnlyList<Item> items, int selectedIndex)
        {
            Config = config;
            Items = items;
            SelectedIndex = selectedIndex;
        }

        public Config Config { get; }
        public IReadOnlyList<Item> Items { get; }
        public int DataCount => Items.Count;

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnSelectedIndexChanged?.Invoke(value);
            }
        }

        public event Action<int> OnSelectedIndexChanged;
        
        public void First()
        {
            if (SelectedIndex > 0)
                SelectedIndex = 0;
        }
        
        public void Last()
        {
            if (SelectedIndex < DataCount - 1)
                SelectedIndex = DataCount - 1;
        }
        
        public void Next()
        {
            if (SelectedIndex < DataCount - 1)
                SelectedIndex++;
        }
        
        public void Previous()
        {
            if (SelectedIndex > 0)
                SelectedIndex--;
        }

        public void Select(int index)
        {
            if (index < 0 || index >= DataCount)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            
            SelectedIndex = index;
        }
    }
}