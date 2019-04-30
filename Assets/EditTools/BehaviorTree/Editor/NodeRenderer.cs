using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BTEditor.Task;

namespace BTEditor
{
	public class NodeRenderer
	{

		Texture2D nodeTexture;
		Texture2D nodeDebugTexture;
		Texture2D shadowTexture;
		Color edgeColor = Color.white;
		Color shadowColor = new Color(0f, 0f, 0f, 0.1f);

		GUIStyle textStyle;


		// Selection
		Texture2D selectionTexture;
		Color selColor = new Color(0f, 162f / 255, 232f / 255, 0.5f);
		float selMargin = 2f;
		float selWidth = 2f;

		private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

		public static float Width { get { return GridRenderer.step.x * 6; } }
		public static float Height { get { return GridRenderer.step.y * 6; } }

		public void Draw(BTNode node, bool selected)
		{
			float shadowOffset = 4;

			// Edge
			if (node.parent != null)
			{
				// Shadow
				Vector2 offset = new Vector2(shadowOffset, shadowOffset);
				DrawEdge(node.parent.editorPosition + offset, node.editorPosition + offset, Width, Height, shadowColor);

				// Line
				DrawEdge(node.parent.editorPosition, node.editorPosition, Width, Height, edgeColor);
			}

			// Node Shadow

			Rect nodeRect = new Rect(node.editorPosition.x, node.editorPosition.y, Width, Height);
			Rect shadowRect = new Rect(nodeRect.x + shadowOffset, nodeRect.y + shadowOffset, nodeRect.width, nodeRect.height);

			if (shadowTexture == null)
			{
				shadowTexture = new Texture2D(1, 1);
				shadowTexture.hideFlags = HideFlags.DontSave;
				shadowTexture.SetPixel(0, 0, shadowColor);
				shadowTexture.Apply();
			}

			GUI.DrawTexture(shadowRect, shadowTexture);
			
			// Node
			if (nodeTexture == null)
			{
				Color colA = new Color(152f / 255, 152f / 255, 152f / 255);
				Color colB = new Color(183f / 255, 183f / 255, 183f / 255);

				nodeTexture = new Texture2D(1, (int)Height);
				nodeTexture.hideFlags = HideFlags.DontSave;
				for (int y = 0; y < Height; y++)
				{
					nodeTexture.SetPixel(0, y, Color.Lerp(colA, colB, y / Height));
				}
				nodeTexture.Apply();
			}
			GUI.DrawTexture(nodeRect, nodeTexture);

			// Icons
			DrawNodeIcon(nodeRect, node);

			// Debug status
			// DrawStatusIcon(nodeRect, node);
			DrawLabel(nodeRect, node);

			// Selection highlight
			if (selected)
			{
				if (selectionTexture == null)
				{
					selectionTexture = new Texture2D(1, 1);
					selectionTexture.hideFlags = HideFlags.DontSave;
					selectionTexture.SetPixel (0, 0, selColor);
					selectionTexture.Apply();
				}

				float mbOffset = selMargin + selWidth; // Margin + Border offset
				GUI.DrawTexture(new Rect(nodeRect.x - mbOffset, nodeRect.y - mbOffset, nodeRect.width + mbOffset * 2, selWidth), selectionTexture); // Top
				GUI.DrawTexture(new Rect(nodeRect.x - mbOffset, nodeRect.y - selMargin, selWidth, nodeRect.height + selMargin * 2), selectionTexture); // Left
				GUI.DrawTexture(new Rect(nodeRect.x + nodeRect.width + selMargin, nodeRect.y - selMargin, selWidth, nodeRect.height + selMargin * 2), selectionTexture); // Right
				GUI.DrawTexture(new Rect(nodeRect.x - mbOffset, nodeRect.y + nodeRect.height + selMargin, nodeRect.width + mbOffset * 2, selWidth), selectionTexture); // Top
			}
		}

		// private void DrawStatusIcon(Rect nodeRect, Node node)
		// {
		// 	// EditorGUI.LabelField(new Rect(nodeRect.x, nodeRect.y + 58f, nodeRect.width, nodeRect.height), node.lastTick.ToString ());
		// 	if (node.lastStatus != null && BTEditorManager.Instance.BehaviorTree.TotalTicks == node.lastTick)
		// 	{
		// 		string status = node.lastStatus.ToString();

		// 		if (!textures.ContainsKey (status))
		// 		{
		// 			Texture2D tex = (Texture2D) EditorGUIUtility.Load ("BTEditor/Status/"+status+".png");
		// 			if (tex == null)
		// 			{
		// 				// Debug.LogWarning (status + ".png not found");
		// 				return;
		// 			}
		// 			tex.hideFlags = HideFlags.DontSave;
		// 			textures.Add (status, tex);
		// 		}

		// 		Rect statusRect = new Rect(nodeRect.x, nodeRect.y, 32f, 32f);
		// 		GUI.DrawTexture(statusRect, textures[status]);
		// 	}
		// }

		private void DrawNodeIcon(Rect nodeRect, BTNode node)
		{
			int width = NearestPowerOfTwo (nodeRect.width);
			int height = NearestPowerOfTwo (nodeRect.height);
			float xOffset = (nodeRect.width - width) / 2;
			float yOffset = (nodeRect.height - height) / 2;
			Rect iconRect = new Rect(nodeRect.x + xOffset, nodeRect.y + yOffset, width, height);

			string nodeName = node.GetType().Name;			
			if (!textures.ContainsKey(nodeName))
			{
				TaskIconAttribute[] attrs = node.GetType().GetCustomAttributes(
					typeof(TaskIconAttribute), true) as TaskIconAttribute[];
				foreach (var attr in attrs)
				{
					Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(attr.name);
					if (tex == null)
					{
						// Debug.LogWarning (nodeName + ".png not found");
						return;
					}
					tex.hideFlags = HideFlags.DontSave;
					textures.Add(nodeName, tex);

					break;
				}
			}

			if (textures.ContainsKey(nodeName))
			{
				GUI.DrawTexture (iconRect, textures[nodeName]);
			}
		}

		int NearestPowerOfTwo(float value)
		{
			int result = 1;
			do
			{
				result = result << 1;
			} while (result << 1 < value);

			return result;
		}

		public static void DrawEdge(Vector2 start, Vector2 end, float width, float height, Color color)
		{
			float offset = width / 2;
			Vector3 startPos = new Vector3(start.x + offset, start.y + height, 0);
			Vector3 endPos = new Vector3(end.x + offset, end.y, 0);
			Vector3 startTan = startPos + Vector3.up * GridRenderer.step.x * 2;
			Vector3 endTan = endPos + Vector3.down * GridRenderer.step.x * 2;
			Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 4);
		}

		public Rect rectForNode(BTNode node)
		{
			return new Rect(node.editorPosition.x, node.editorPosition.y, Width, Height);
		}

		// 绘制节点的特殊信息
		void DrawLabel(Rect nodeRect, BTNode node)
		{
			// Title
			string title = node.GetType().Name;
			// Is need to replace name to comment
			if (node.replaceShowName && !string.IsNullOrEmpty(node.comment))
			{
				title = node.comment;
			}

			Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(title));
			float x = node.editorPosition.x + (Width / 2) - (textSize.x / 2);
			Rect titleRect = new Rect(x, node.editorPosition.y + Height - (Height / 5),
				textSize.x + 10, textSize.y);
			if (textStyle == null)
			{
				textStyle = new GUIStyle();
				textStyle.normal.textColor = Color.black;
			}

			// Title
			EditorGUI.LabelField(titleRect, title, textStyle);

			// Debug ID
			EditorGUI.LabelField(nodeRect, node.debugId, textStyle);
		}
	}
}

