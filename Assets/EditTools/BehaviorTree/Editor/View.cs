using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BTEditor.Task;

namespace BTEditor
{
	enum Mode { NodeAction, CanvasAction, DragNode, ConnectParent, ConnectChild, InvokeMenu, None }

	public class View
	{
		List<int> mSelectedNodes = new List<int>();

		GridRenderer mGridRenderer;
		NodeRenderer mNodeRenderer;
		Rect mCanvas;
		Vector2 mScrollPoint { get; set; }

		BTEditorWindow owner;

		Mode mCurMode = Mode.None;

		Vector2 mMouseStartPos = Vector2.zero;

		#region Zoom

		const float ZoomMin = 0.5f;
		const float ZoomMax = 1.5f;
		
		float zoomScale = 1.0f;

		#endregion Zoom

		#region Pickup

		Material mPickupMat;
		Color mPickupColor;
		Color mPickupEdgeColor;
		Vector3 mPickupStartPos;
		bool mPickingup;

		#endregion Pickup

		public View(BTEditorWindow owner)
		{
			this.owner = owner;
			mCanvas = new Rect(0, 0, owner.position.width, owner.position.height);

			BTEditorManager.onPaste += OnPaste;
		}

		~View()
		{
			BTEditorManager.onPaste -= OnPaste;
		}

		/// <summary>
		/// 切换文件时重置数据
		/// </summary>
		public void Reset()
		{
			zoomScale = 1.0f;
			mScrollPoint = Vector2.zero;
		}

		void InitPickupMat()
		{
			if (mPickupMat == null)
			{
				mPickupMat = new Material(Shader.Find("UI/Default"));
				mPickupColor = new Color(110f / 255, 124f / 255, 145f / 255, 120f / 255);
				mPickupEdgeColor = new Color(1f, 1f, 1f, 0.5f);
				mPickupStartPos = Vector3.zero;
				mPickingup = false;
			}
		}

		void DrawNodes(Dictionary<int, BTNode> nodeDic)
		{
			if (mNodeRenderer == null) mNodeRenderer = new NodeRenderer();

			foreach (var kvp in nodeDic)
			{
				if (kvp.Value != null)
				{
					mNodeRenderer.Draw(kvp.Value, mSelectedNodes.Contains(kvp.Key));
				}
			}
		}

		public void Draw(Rect position)
		{
			GUI.Label(new Rect(0f, 0f, position.width, position.height),
				"WorkDir: " + BTEditorManager.SelectAsset.GetFullpath());

			// 缩放
			Rect zoomedPosition = EditorZoomArea.Begin(zoomScale, position);
			// 滚动
			mScrollPoint = GUI.BeginScrollView(new Rect(0f, 0f,
				zoomedPosition.width, zoomedPosition.height), mScrollPoint, mCanvas);

			#region Core logic

			// 输入处理
			HandleMouseEvents(zoomedPosition, BTEditorManager.SelectTree.nodeDic);

			// 绘制背景网格
			if (mGridRenderer == null) mGridRenderer = new GridRenderer();
			mGridRenderer.Draw(mCanvas);

			// 绘制节点
			DrawNodes(BTEditorManager.SelectTree.nodeDic);
			if (mCurMode == Mode.ConnectChild || mCurMode == Mode.ConnectParent)
			{
				DrawConnectionLine();
			}

			// 绘制鼠标框选范围
			DrawPickupRect();

			#endregion Core logic

			GUI.EndScrollView();
			EditorZoomArea.End();
		}

		void DrawConnectionLine()
		{
			if (mSelectedNodes.Count < 0) return;

			BTNode contextNode = BTEditorManager.GetNodeByID(mSelectedNodes[0]);
			if (contextNode == null) return;
			
			Vector3 startPos = Vector3.zero;
			Vector3 startTan = Vector3.zero;
			Vector2 mousePos = Event.current.mousePosition;
			Vector3 endPos = new Vector3(mousePos.x, mousePos.y, 0);
			Vector3 endTan = Vector3.zero;

			if (mCurMode == Mode.ConnectParent)
			{
				startPos = new Vector3(contextNode.editorPosition.x + (NodeRenderer.Width / 2), contextNode.editorPosition.y, 0);
				startTan = startPos + Vector3.down * GridRenderer.step.x * 2;
				endTan = endPos + Vector3.up * GridRenderer.step.x * 2;
			}
			else if (mCurMode == Mode.ConnectChild)
			{
				startPos = new Vector3(contextNode.editorPosition.x + (NodeRenderer.Width / 2), contextNode.editorPosition.y + NodeRenderer.Height, 0);
				startTan = startPos + Vector3.up * GridRenderer.step.x * 2;
				endTan = endPos + Vector3.down * GridRenderer.step.x * 2;
			}

			Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 4);
		}

		// Returns true if needs a repaint
		void HandleMouseEvents(Rect position, Dictionary<int, BTNode> nodeDic)
		{
			Event e = Event.current;

			// MouseDown //
			// Identify the control being clicked
			if (Event.current.type == EventType.MouseDown)
			{
				// Do nothing for MouseDown on the scrollbar, if present
				if (e.mousePosition.x >= position.width + mScrollPoint.x
					|| e.mousePosition.y >= position.height + mScrollPoint.y)
				{
					mCurMode = Mode.None;
				}
				// MouseDown in the canvas, check if in a node or on background
				else
				{
					// Store the mouse position
					mMouseStartPos = e.mousePosition;

					// Loop through nodes and check if their rects contain the mouse position
					BTNode contextNode = null;
					BTNode tempNode = null;
					foreach (var kvp in nodeDic)
					{
						tempNode = kvp.Value;
						if (tempNode != null && mNodeRenderer.rectForNode(tempNode).Contains(mMouseStartPos))
						{
							// Connect a parent to a child
							if (mCurMode == Mode.ConnectChild)
							{
								BTEditorManager.Connect(owner.connectOrigin,
									tempNode.GetInstanceID());
								// editorWindow.wantsMouseMove = false;
								mCurMode = Mode.None;
								break;
							}
							// Connect a child to a parent
							else if (mCurMode == Mode.ConnectParent)
							{
								BTEditorManager.Connect(tempNode.GetInstanceID(),
									owner.connectOrigin);
								// editorWindow.wantsMouseMove = false;
								mCurMode = Mode.None;
								break;
							}
							// Perform a node action at key up
							else
							{
								mCurMode = Mode.NodeAction;
								contextNode = tempNode;
							}
						}
					}

					// Cancel the connection
					if (mCurMode == Mode.ConnectParent || mCurMode == Mode.ConnectChild)
					{
						// editorWindow.wantsMouseMove = false;
						mCurMode = Mode.None;
					}

					// MouseDown on the canvas background enables panning the view
					if (mCurMode == Mode.None && contextNode == null)
					{
						mCurMode = Mode.CanvasAction;
					}

					SelectNode(contextNode);
				}

				return;
			}

			// Mouse Up //
			// MouseUp resets the current interaction mode to None
			if (e.type == EventType.MouseUp)
			{
				mPickingup = false;
				
				// Context Menu
				if (IsRightMouseBtn())
				{
					if (mCurMode == Mode.NodeAction)
					{
						owner.ShowContextMenu(e.mousePosition,
							EditorZoomArea.GetScrollOffset(zoomScale, mScrollPoint), mSelectedNodes);
					}
					else if (mCurMode == Mode.CanvasAction)
					{
						owner.ShowContextMenu(e.mousePosition,
							EditorZoomArea.GetScrollOffset(zoomScale, mScrollPoint), null);
					}
				}
				// Resize canvas after a drag
				else if (mCurMode == Mode.DragNode)
				{
					
					BTEditorManager.Dirty();
				}
				
				mCurMode = Mode.None;

				return;
			}

			// Mouse Drag //
			if (e.type == EventType.MouseDrag)
			{
				if (IsLeftMouseBtn())
				{
					if (mCurMode == Mode.CanvasAction)
					{
						if (e.modifiers == EventModifiers.Alt)
						{
							mPickingup = true;
							mPickupStartPos = mMouseStartPos;
						}
						else if (e.modifiers == EventModifiers.None)
						{
							mScrollPoint -= (e.delta / zoomScale);
						}
					}
					// Switch to node dragging mode
					else if (mCurMode == Mode.NodeAction)
					{
						if (mSelectedNodes.Count > 0 && e.modifiers != EventModifiers.Control)
						{
							Vector2 mousePos = e.mousePosition;
							float deltaX = Mathf.Abs(mousePos.x - mMouseStartPos.x);
							float deltaY = Mathf.Abs(mousePos.y - mMouseStartPos.y);

							// Ignore mouse drags inside nodes lesser than the grid step. These would be rounded,
							// and make selecting a node slightly more difficult.
							if (deltaX >= GridRenderer.step.x || deltaY >= GridRenderer.step.y)
							{
								mCurMode = Mode.DragNode;
							}
						}
					}
					// Drag a node
					if (mCurMode == Mode.DragNode)
					{
						// 距离Node中心位置的移动距离
						DragNodes(mSelectedNodes);
					}
				}

				return;
			}

			// 鼠标移出窗口
			if (e.type == EventType.MouseLeaveWindow)
			{
				if (mPickingup)
				{
					mPickingup = false;
				}

				return;
			}
			
			// Zoom
			if (e.type == EventType.ScrollWheel)
			{
				if (e.modifiers == EventModifiers.Alt)
				{
					Vector2 delta = e.delta;
					float zoomDelta = -delta.y / 150.0f;
					zoomScale += zoomDelta;
					zoomScale = Mathf.Clamp(zoomScale, ZoomMin, ZoomMax);

					e.Use();
				}

				return;
			}

			// 按键操作
			if (e.type == EventType.KeyDown)
			{
				if (e.control)
				{
					// 复制
					if (e.keyCode == KeyCode.C)
					{
						BTEditorManager.Copy(mSelectedNodes.ToArray());
						return;
					}

					// 粘贴
					if (e.keyCode == KeyCode.V)
					{
						Vector2 to = BTEditorManager.CopyFromPos;
						to.x += 100f;
						BTEditorManager.Paste(to);

						return;
					}
				}

				// 删除
				if (e.keyCode == KeyCode.Delete)
				{
					BTEditorManager.Delete(mSelectedNodes.ToArray());
					return;
				}
			}
		}

		/// <summary>
		/// 选中节点
		/// </summary>
		/// <param name="selected"></param>
		private void SelectNode(BTNode selected)
		{
			// 按住Ctrl时复选节点
			if (Event.current.modifiers == EventModifiers.Control && IsLeftMouseBtn())
			{
				SelectMultipleNode(selected);
			}
			// 单击节点
			else
			{
				SelectSingleNode(selected);
			}
		}

		/// <summary>
		/// 单选节点
		/// </summary>
		/// <param name="selected"></param>
		private void SelectSingleNode(BTNode selected)
		{
			if (selected != null)
			{
				int instanceID = selected.GetInstanceID();
				if (mSelectedNodes.Contains(instanceID))
				{
					// 单击已复选的目标不处理，用于后续的拖拽，复制操作
					return;
				}
				else
				{
					// 单选新目标
					mSelectedNodes.Clear();
					mSelectedNodes.Add(instanceID);
				}
			}
			else
			{
				mSelectedNodes.Clear();
			}

			RefreshSelection();
		}

		/// <summary>
		/// 多选节点
		/// </summary>
		/// <param name="selected"></param>
		private void SelectMultipleNode(BTNode selected)
		{
			if (selected != null)
			{
				int instanceID = selected.GetInstanceID();
				if (mSelectedNodes.Contains(instanceID))
				{
					for (int i = 0; i < mSelectedNodes.Count; ++i)
					{
						if (mSelectedNodes[i].Equals(instanceID))
						{
							mSelectedNodes.RemoveAt(i);
							break;
						}
					}
				}
				else
				{
					mSelectedNodes.Add(instanceID);
				}
			}
			else
			{
				mSelectedNodes.Clear();
			}

			RefreshSelection();
		}

		private void RefreshSelection()
		{
			if (mSelectedNodes == null)
			{
				return;
			}

			int count = mSelectedNodes.Count;
			UnityEngine.Object[] nodeArray = new UnityEngine.Object[count];
			for (int i = 0; i < count; ++i)
			{
				nodeArray[i] = BTEditorManager.GetNodeByID(mSelectedNodes[i]);
			}

			Selection.objects = nodeArray;
		}

		private void DragNodes(List<int> nodes)
		{
			if (Application.isPlaying || nodes.Count <= 0)
			{
				return;
			}

			Vector2 step = GetDragStep();

			// 只有单节点时才允许连带子节点移动
			BTNode tempNode = null;
			if (nodes.Count == 1 && Event.current.shift)
			{
				tempNode = BTEditorManager.GetNodeByID(nodes[0]);
				DragWithChildren(tempNode, step);
			}
			else
			{
				// 只移动选中的节点
				foreach (var nodeID in nodes)
				{
					tempNode = BTEditorManager.GetNodeByID(nodeID);
					if (tempNode == null)
					{
						continue;	
					}

					tempNode.editorPosition += step;
				}
			}

			mMouseStartPos += step;
		}

		private Vector2 GetDragStep()
		{
			// 按照GridRenderer.step的步长移动节点，便于对齐
			Vector2 offset = Event.current.mousePosition - mMouseStartPos;
			Vector2 step = new Vector2(offset.x - (offset.x % GridRenderer.step.x),
				offset.y - (offset.y % GridRenderer.step.y));

			return step;
		}

		private void DragWithChildren(BTNode self, Vector2 step)
		{
			self.editorPosition += step;

			for (int i = 0; i < self.ChildCount; i++)
			{
				DragWithChildren(self.Children[i], step);
			}
		}

		/// <summary>
		/// 重设画布大小
		/// </summary>
		/// <param name="firstResize">初次设置画布时不修改滑动条位置</param>
		public void ResizeCanvas(bool firstResize)
		{
			Vector2 zoomedPosition = new Vector2(owner.position.width / zoomScale,
				owner.position.height / zoomScale);
			Rect newCanvas = new Rect(0, 0, zoomedPosition.x, zoomedPosition.y);

			Vector2 baseRoot = Vector2.zero;

			foreach (var kvp in BTEditorManager.SelectTree.nodeDic)
			{
				float x = kvp.Value.editorPosition.x;
				float y = kvp.Value.editorPosition.y;

				// Evaluate min position
				if (newCanvas.xMin > x) newCanvas.xMin = x;
				if (newCanvas.yMin > y) newCanvas.yMin = y;

				// Evaluate size
				float xMax = x + NodeRenderer.Width;
				if (xMax > newCanvas.xMax)
				{
					newCanvas.xMax = xMax;
				}
				float yMax = y + NodeRenderer.Height;
				if (yMax > newCanvas.yMax)
				{
					newCanvas.yMax = yMax;
				}

				if (kvp.Value is BaseRoot)
				{
					baseRoot = new Vector2(xMax, yMax);
				}
			}

			// 扩大画布范围，便于拖拽
			float offset = 300 / zoomScale;
			newCanvas.xMax += offset;
			newCanvas.yMax += offset;

			// Evaluate new scroll point
			float xScroll = 0f;
			float yScroll = 0f;
			if (firstResize)
			{
				if (baseRoot.x > zoomedPosition.x)
				{
					xScroll = baseRoot.x;
				}
				if (baseRoot.y > zoomedPosition.y)
				{
					yScroll = baseRoot.y;
				}
				mScrollPoint = new Vector2(xScroll / 2, yScroll / 2);
			}
			else
			{
				xScroll = mCanvas.xMin - newCanvas.xMin;
				if (xScroll < 0) xScroll = 0f;
				yScroll = mCanvas.yMin - newCanvas.yMin;
				if (yScroll < 0) yScroll = 0f;
				mScrollPoint += new Vector2(xScroll, yScroll);
			}

			mCanvas = newCanvas;
		}

		public void StartConnectParent()
		{
			// editorWindow.wantsMouseMove = true;
			// contextNode = node;
			mCurMode = Mode.ConnectParent;
		}

		public void StartConnectChild()
		{
			// editorWindow.wantsMouseMove = true;
			// contextNode = node;
			mCurMode = Mode.ConnectChild;
		}

		void DrawPickupRect()
		{
			if (!mPickingup) return;

			if (mPickupMat == null) InitPickupMat();
			if (mPickupMat != null)
			{
				// Prepare data
				Vector3 endPos = Event.current.mousePosition;//鼠标当前位置
				Vector3[] points = new Vector3[4];
				points[0] = new Vector3(mPickupStartPos.x, mPickupStartPos.y, 0);
				points[1] = new Vector3(endPos.x, mPickupStartPos.y, 0);
				points[2] = new Vector3(endPos.x, endPos.y, 0);
				points[3] = new Vector3(mPickupStartPos.x, endPos.y, 0);
				// Draw
				Handles.DrawSolidRectangleWithOutline(points, mPickupColor, mPickupEdgeColor);

				CheckSelection(mPickupStartPos, endPos);
			}
		}

		void CheckSelection(Vector2 start, Vector2 end)
		{
			// 确保max的坐标始终大于min的坐标
			Vector2 min = Vector3.zero;
			Vector2 max = Vector3.zero;
			if (start.x < end.x)
			{
				min.x = start.x;
				max.x = end.x;
			}
			else
			{
				min.x = end.x;
				max.x = start.x;
			}
			
			if (start.y < end.y)
			{
				min.y = start.y;
				max.y = end.y;
			}
			else
			{
				min.y = end.y;
				max.y = start.y;
			}
	
			mSelectedNodes.Clear();
			foreach (var node in BTEditorManager.SelectTree.nodeDic.Values)
			{
				Vector2 nodePos = node.editorPosition;

				if (nodePos.x + NodeRenderer.Width < min.x || nodePos.x > max.x
					|| nodePos.y + NodeRenderer.Height < min.y || nodePos.y > max.y)
				{
					continue;
				}

				mSelectedNodes.Add(node.GetInstanceID());
			}

			RefreshSelection();
		}

		/// <summary>
		/// 是否为鼠标左键的事件
		/// </summary>
		/// <returns></returns>
		bool IsLeftMouseBtn()
		{
			return Event.current.button == 0;
		}

		/// <summary>
		/// 是否为鼠标右键的事件
		/// </summary>
		/// <returns></returns>
		bool IsRightMouseBtn()
		{
			return Event.current.button == 1;
		}

		/// <summary>
		/// 粘贴完成回调，选中新的生成的元素
		/// </summary>
		void OnPaste(int[] list)
		{
			if (list.Length <= 0) return;

			mSelectedNodes.Clear();
			foreach (var id in list)
			{
				mSelectedNodes.Add(id);
			}
		}
	}
}