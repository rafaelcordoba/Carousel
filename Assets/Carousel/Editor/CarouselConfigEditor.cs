using System.Collections.Generic;
using System.Linq;
using Carousel.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Carousel.Editor
{
    [CustomEditor(typeof(CarouselConfig))]
    public class CarouselConfigEditor : UnityEditor.Editor
    {
        private IReadOnlyList<Carousel3D> _carouselsMatchingTarget;
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

            _carouselsMatchingTarget ??= FindObjectsOfType<Carousel3D>()
                .Where(c => c.Config == _config).ToList();

            foreach (var carousel in _carouselsMatchingTarget)
            {
                carousel.InitializeAnimator();
                carousel.UpdateVisibleViews();
            }
        }
    }
}