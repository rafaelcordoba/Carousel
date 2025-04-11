using System;
using System.Collections.Generic;
using Presentation;
using UnityEngine;

namespace Samples.Data
{
    [CreateAssetMenu(fileName = "SampleData", menuName = "Carousel/New Sample Data")]
    public class SampleData : ScriptableObject
    {
        [SerializeField] private AbstractItemView defaultPrefab;
        [SerializeField] private List<Data> datas;
        [SerializeField] private List<DataWithIcon> dataWithIcon;

        public AbstractItemView DefaultPrefab => defaultPrefab;
        public List<Data> Datas => datas;
        public List<DataWithIcon> DataIcons => dataWithIcon;

        [Serializable]
        public class Data
        {
            public AbstractItemView customPrefab;
            public string title;
            public string description;
        }

        [Serializable]
        public class DataWithIcon : Data
        {
            public Sprite icon;
        }
    }
}