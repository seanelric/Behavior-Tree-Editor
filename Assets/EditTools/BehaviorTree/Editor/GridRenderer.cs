using UnityEngine;
using UnityEditor;
using System.Collections;

namespace BTEditor
{
	public class GridRenderer
	{
		Texture2D gridTex;

		public static int width { get { return 120; } }
		public static int height { get { return 120; } }
		public static Vector2 step { get { return new Vector2(width / 10, height / 10); } }
		
		// Generates a single tile of the grid texture
		void GenerateGrid()
		{
			gridTex = new Texture2D(width, height);
			gridTex.hideFlags = HideFlags.DontSave;
			
			float weight = 93f / 255;
			Color bg = new Color(weight, weight, weight);
			Color dark = Color.Lerp(bg, Color.black, 0.15f);
			Color light = Color.Lerp(bg, Color.black, 0.05f);
			
			for (int x = 0; x < width; x ++) {

				for (int y = 0; y < height; y ++)
				{	
					// Left and Top edges, dark color
					if (x == 0 || y == 0)
					{
						gridTex.SetPixel(x, y, dark);
					}
					// Finer grid color
					else if (x % step.x == 0 || y % step.y == 0)
					{
						gridTex.SetPixel(x, y, light);
					}
					// Background
					else gridTex.SetPixel(x, y, bg);
				}
			}
			
			gridTex.Apply();
		}
		
		public void Draw(Rect canvas)
		{
			if (!gridTex) GenerateGrid();
			
			for (float x = canvas.x; x < canvas.x + canvas.width; x += gridTex.width)
			{
				for (float y = canvas.y; y < canvas.y + canvas.height; y += gridTex.height)
				{
					GUI.DrawTexture(new Rect(x, y, gridTex.width, gridTex.height), gridTex);
				}
			}
		}
	}
}