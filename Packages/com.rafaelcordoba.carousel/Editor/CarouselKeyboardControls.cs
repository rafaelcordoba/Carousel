using Carousel.Runtime;
using UnityEditor;
using UnityEngine;

namespace Carousel.Editor
{
    [InitializeOnLoad]
    public class CarouselKeyboardControls
    {
        static CarouselKeyboardControls()
        {
            EditorApplication.update += HandleInput;
        }

        private static void HandleInput()
        {
            if (!EditorApplication.isPlaying)
                return;

            HandleNumberKeys();
            HandleNumpadKeys();
        }

        private static void HandleNumberKeys()
        {
            for (var key = KeyCode.Alpha0; key <= KeyCode.Alpha9; key++)
            {
                if (!Input.GetKeyDown(key))
                    continue;

                var index = (int)key - (int)KeyCode.Alpha0;
                ProcessIndexSelection(index);
                break;
            }
        }

        private static void HandleNumpadKeys()
        {
            for (var key = KeyCode.Keypad0; key <= KeyCode.Keypad9; key++)
            {
                if (!Input.GetKeyDown(key))
                    continue;

                var index = (int)key - (int)KeyCode.Keypad0;
                ProcessIndexSelection(index);
                break;
            }
        }

        private static void ProcessIndexSelection(int index)
        {
            var carousels = Object.FindObjectsOfType<Carousel3D>();
            if (carousels == null || carousels.Length == 0) return;

            foreach (var carousel in carousels)
            {
                if (!carousel)
                    continue;

                carousel.Select(Mathf.Min(index, carousel.DataCount - 1));
            }
        }
    }
}