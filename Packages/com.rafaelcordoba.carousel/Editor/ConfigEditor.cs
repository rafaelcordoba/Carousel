using Domain;
using Presentation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Carousel.Editor
{
    [CustomEditor(typeof(Config))]
    public class ConfigEditor : UnityEditor.Editor
    {
        private CarouselViewStatic _carouselView;
        private Config _config;

        public override VisualElement CreateInspectorGUI()
        {
            _config = (Config)target;
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

            if (!_carouselView)
                _carouselView = FindFirstObjectByType<CarouselViewStatic>();
            
            if (!_carouselView || _carouselView.Config != _config)
                return;
            
            _carouselView.Refresh();
        }
    }
}