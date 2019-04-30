using UnityEngine;

namespace BTEditor
{
    public class EditorZoomArea
    {
        // 自定义路径文本高度
        public static Vector2 WorkDirOffset = new Vector2(0, 14f);

        // Unity默认的窗口标题栏坐标偏移量
        static Vector2 EditorWindowTabOffset = new Vector2(0f, 21f);

        static Matrix4x4 _prevGuiMatrix;

        public static Rect Begin(float zoomScale, Rect screenCoordsArea)
        {
            GUI.EndGroup();

			Rect clippedArea = new Rect(0f, 0f, screenCoordsArea.width, screenCoordsArea.height - WorkDirOffset.y);
            clippedArea = clippedArea.ScaleSizeBy(1f / zoomScale, clippedArea.TopLeft());
            clippedArea.y += EditorWindowTabOffset.y + WorkDirOffset.y;
			GUI.BeginGroup(clippedArea);

			_prevGuiMatrix = GUI.matrix;
			Matrix4x4 translation = Matrix4x4.TRS(new Vector3(clippedArea.xMin, clippedArea.yMin, 0), Quaternion.identity, Vector3.one);
			Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
			GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

			return clippedArea;
        }

        public static void End()
        {
            GUI.matrix = _prevGuiMatrix;
        	GUI.EndGroup();
        	GUI.BeginGroup(new Rect(0f, EditorWindowTabOffset.y, Screen.width, Screen.height));
        }

        /// <summary>
        /// 获取缩放后的实际滚动条偏移量，用于菜单位置的显示
        /// </summary>
        /// <param name="zoomScale"></param>
        /// <param name="scrollPoint"></param>
        /// <returns></returns>
        public static Vector2 GetScrollOffset(float zoomScale, Vector2 scrollPoint)
        {
            float scale = 1 / zoomScale - 1;
            return scale * (scrollPoint - EditorWindowTabOffset - WorkDirOffset);
        }
    }
}
