#nullable enable
using UnityEngine;

namespace YuzeToolkit.GUITool
{
    public static class FGUI
    {
        public static Rect ByWidth(this ref Rect rect, float width)
        {
            var returnRect = new Rect(rect.x, rect.y, width, rect.height);
            rect = new Rect(rect.x + width, rect.y, rect.width - width, rect.height);
            return returnRect;
        }

        public static Rect ByHeight(this ref Rect rect, float height)
        {
            var returnRect = new Rect(rect.x, rect.y, rect.width, height);
            rect = new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
            return returnRect;
        }

        public static Rect ByWidth(this ref Rect rect, float width, float height)
        {
            var returnRect = new Rect(rect.x, rect.y, width, height);
            rect = new Rect(rect.x + width, rect.y, rect.width - width, rect.height);
            return returnRect;
        }
        
        public static Rect ByHeight(this ref Rect rect, float width, float height)
        {
            var returnRect = new Rect(rect.x, rect.y, width, height);
            rect = new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
            return returnRect;
        }
    }
}