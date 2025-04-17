using UnityEngine;

namespace Potman.Common.Utilities
{
    public static class CanvasExtensions
    {
        public static Vector2 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null)
        {
            camera = camera ? camera : Camera.main!;
            var viewportPosition = camera.WorldToViewportPoint(worldPosition);
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector2 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
        {
            var viewportPosition = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector2 ViewportToCanvasPosition(this Canvas canvas, Vector2 viewportPosition)
        {
            var centerBasedViewPortPosition = viewportPosition - new Vector2(0.5f, 0.5f);
            var canvasRect = canvas.GetComponent<RectTransform>();
            var scale = canvasRect.sizeDelta;
            return Vector2.Scale(centerBasedViewPortPosition, scale);
        }
    }
}
