using UnityEngine;

namespace Assets.Scripts.Utility
{
    public static class CanvasMouseFollower
    {
        public static Vector3 GetPointerPosOnCanvas(Canvas canvas, Vector2 pointerPos)
        {
            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceCamera:
                {
                    Vector2 pos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, pointerPos, canvas.worldCamera, out pos);
                    return canvas.transform.TransformPoint(pos);
                }
                case RenderMode.ScreenSpaceOverlay:
                    return Input.mousePosition;
                default:
                {
                    if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector3 globalMousePos))
                    {
                        return globalMousePos;
                    }

                    break;
                }
            };

            return Vector2.zero;
        }

    }
}
