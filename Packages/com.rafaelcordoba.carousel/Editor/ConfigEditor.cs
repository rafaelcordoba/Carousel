using Carousel.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Carousel.Editor
{
    [CustomEditor(typeof(Config))]
    public class ConfigEditor : UnityEditor.Editor
    {
        private CarouselStatic _carousel;
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

            if (!_carousel)
                _carousel = FindFirstObjectByType<CarouselStatic>();
            
            if (!_carousel || _carousel.Config != _config)
                return;
            
            _carousel.Refresh();
        }
    }
}