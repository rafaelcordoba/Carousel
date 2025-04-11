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
                SelectedIndexBeforeChange?.Invoke(_selectedIndex, value);
                _selectedIndex = value;
                SelectedIndexChanged?.Invoke(value);
            }
        }

        public event Action<int, int> SelectedIndexBeforeChange;
        public event Action<int> SelectedIndexChanged;

        public void First()
        {
            if (SelectedIndex > 0)
                SelectedIndex = 0;
        }
        
        public void Last()
        {
            if (SelectedIndex < Items.Count - 1)
                SelectedIndex = Items.Count - 1;
        }
        
        public void Next()
        {
            if (SelectedIndex < Items.Count - 1)
                SelectedIndex++;
        }
        
        public void Prev()
        {
            if (SelectedIndex > 0)
                SelectedIndex--;
        }

        public void Select(int index)
        {
            if (!IsIndexValid(index))
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            
            SelectedIndex = index;
        }

        public bool IsIndexValid(int index) 
            => index >= 0 && index < Items.Count;
    }
}