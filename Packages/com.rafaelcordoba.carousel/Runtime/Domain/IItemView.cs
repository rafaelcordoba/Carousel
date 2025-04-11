using UnityEngine;

namespace Domain
{
    public interface IItemView
    {
        void Initialize(int index, IItemData itemData, ICarouselSelect select);
        GameObject GameObject { get; }
    }
}