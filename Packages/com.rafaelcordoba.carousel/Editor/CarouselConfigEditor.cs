using Carousel.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Carousel.Editor
{
    [CustomEditor(typeof(CarouselConfig))]
    public class CarouselConfigEditor : UnityEditor.Editor
    {
        private Carousel3D _carousel;
        private CarouselConfig _config;

        public override VisualElement CreateInspectorGUI()
        {
            _config = (CarouselConfig)target;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            DrawDefaultInspector();
            if (!EditorGUI.EndChangeCheck())
                return;

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);

            if (!Application.isPlaying)
                return;

            if (!_carousel)
                _carousel = FindFirstObjectByType<Carousel3D>();
            
            if (!_carousel || _carousel.Config != _config)
                return;
            
            _carousel.InitializeAnimator();
            _carousel.UpdateVisibleViews();
        }
    }
}