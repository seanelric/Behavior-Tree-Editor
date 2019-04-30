using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using BTEditor.Task;

namespace BTEditor
{
	public class BTEditorWindow : EditorWindow
	{
		public class MenuAction
		{
			public System.Type nodeType;
			public Vector2 position;
			public List<int> nodeIDs;

			public MenuAction(List<int> nodesVal, Vector2 positionVal = default(Vector2), System.Type nodeTypeVal = null)
			{
				nodeIDs = nodesVal;
				position = positionVal;
				nodeType = nodeTypeVal;
			}
		}
		
		public View view;
		// 连接起始节点
		public int connectOrigin;

		private GUIStyle mNoSelectionStyle;

		// 是否是初次重设画布大小
		bool firstResizeCanvas = true;

		Dictionary<string, System.Type> mAddContextDic = new Dictionary<string, System.Type>();

		[MenuItem("策划工具/AI/打开AI编辑器", false, 210)]
		public static void ShowWindow()
		{
			BTEditorWindow window = EditorWindow.GetWindow<BTEditorWindow>("BehaviorDesigner");
			window.Show();
		}

		void OnEnable()
		{
			mNoSelectionStyle = new GUIStyle();
			mNoSelectionStyle.fontSize = 24;
			mNoSelectionStyle.alignment = TextAnchor.MiddleCenter;

			BTEditorManager.onDisplayChanged += onDisplayChanged;
			BTEditorManager.onNeedRepaint += OnNeedRepaint;
			
			if (view == null) view = new View(this);
		}

		void OnDisable()
		{
			BTEditorManager.onDisplayChanged -= onDisplayChanged;
			BTEditorManager.onNeedRepaint -= OnNeedRepaint;
		}

		void OnGUI()
		{
			if (BTEditorManager.SelectAsset != null)
			{
				view.Draw(position);
				view.ResizeCanvas(firstResizeCanvas);
				firstResizeCanvas = false;

				Repaint();
			}
		}

		void OnFocus()
		{
			InitAddContext();
		}

		void OnSelectionChange()
		{
			Repaint();
		}

		/// <summary>
		/// 当前显示的行为树变化回调
		/// </summary>
		void onDisplayChanged()
		{
			BTEditorManager.CopyFromNodes.Clear();
			if (view != null) view.Reset();
			
			firstResizeCanvas = true;
		}

		/// <summary>
		/// 当前界面需要重绘的回调
		/// </summary>
		void OnNeedRepaint()
		{
			Repaint();
		}

		void DrawNodeWindow(int id)
		{
			GUI.DragWindow();
		}

		// 初始化右键菜单中的添加列表
		void InitAddContext()
		{
			mAddContextDic.Clear();
			
			foreach (var kvp in BTEditorManager.NodeTypes)
			{
				// 使用反射获取属性（在菜单中的名字）
				TaskCategoryAttribute[] attrs = kvp.Value.GetCustomAttributes(
					typeof(TaskCategoryAttribute), false) as TaskCategoryAttribute[];
				foreach (var attr in attrs)
				{
					if (mAddContextDic.ContainsKey(attr.name))
					{
						Debug.LogError("Same category, name =" + attr.name + "type=," + kvp.Value);
						continue;
					}

					mAddContextDic.Add(attr.name, kvp.Value);
					break;
				}
			}
		}

		public void ShowContextMenu(Vector2 mousePos, Vector2 offset, List<int> nodeList)
		{
			if (Application.isPlaying)
			{
				return;
			}

			var menu = new GenericMenu();
			if (nodeList != null && nodeList.Count > 0)
			{
				if (nodeList.Count == 1)
				{
					BTNode node = BTEditorManager.GetNodeByID(nodeList[0]);

					// 添加
					if (node.CanConnectChild)
					{
						foreach (var kvp in mAddContextDic)
						{
							menu.AddItem(new GUIContent("Add Child/" + kvp.Key), false, Add,
								new MenuAction(nodeList, mousePos, kvp.Value));
						}
					}
					else menu.AddDisabledItem(new GUIContent("Add"));

					// Connect/Disconnect Parent
					menu.AddSeparator("");
					if (!(node is BaseRoot) && !(node is Root))
					{
						if (node.parent != null)
							menu.AddItem(new GUIContent("Disconnect from Parent"), false, Unparent, new MenuAction(nodeList));
						else
							menu.AddItem(new GUIContent("Connect to Parent"), false, ConnectParent, new MenuAction(nodeList));
					}

					// Connect Child
					menu.AddSeparator("");
					if (node.CanConnectChild)
						menu.AddItem(new GUIContent("Connect to Child"), false, ConnectChild, new MenuAction(nodeList));
					else
						menu.AddDisabledItem(new GUIContent("Connect to Child"));

					// Copy
					menu.AddSeparator("");
					menu.AddItem(new GUIContent("Copy"), false, Copy, new MenuAction(nodeList, mousePos));

					// Delete
					menu.AddSeparator("");
					menu.AddItem(new GUIContent("Delete"), false, Delete, new MenuAction(nodeList));
				}
				else
				{
					// Copy
					menu.AddSeparator("");
					menu.AddItem(new GUIContent("Copy"), false, Copy, new MenuAction(nodeList, mousePos));

					// Delete
					menu.AddSeparator("");
					menu.AddItem(new GUIContent("Delete"), false, Delete, new MenuAction(nodeList));
				}
			}
			// 右键点击画布
			else
			{
				// 添加
				foreach (var kvp in mAddContextDic)
				{
					menu.AddItem(new GUIContent("Add/" + kvp.Key), false, Add,
						new MenuAction(nodeList, mousePos, kvp.Value));
				}

				// 粘贴
				menu.AddSeparator("");
				if (BTEditorManager.CopyFromNodes.Count > 0)
				{
					menu.AddItem(new GUIContent("Paste"), false, Paste,
						new MenuAction(null, mousePos));
				}
				else
				{
					menu.AddDisabledItem(new GUIContent("Paste"));
				}

				// 保存
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Save"), false, Save, null);
			}

			menu.DropDown(new Rect(mousePos + offset, Vector2.zero));
		}

		// Context Menu actions
		public void Add(object userData)
		{
			MenuAction menuAction = userData as MenuAction;
			int parentID = -1;
			if (menuAction.nodeIDs != null)
			{
				parentID = menuAction.nodeIDs[0];
			}
			
			BTEditorManager.Add(parentID, menuAction.position, menuAction.nodeType);
		}

		public void Unparent(object userData)
		{
			MenuAction menuAction = userData as MenuAction;
			BTEditorManager.Unparent(menuAction.nodeIDs[0]);
		}

		public void ConnectParent(object userData)
		{
			MenuAction menuAction = userData as MenuAction;
			connectOrigin = menuAction.nodeIDs[0];
			view.StartConnectParent();
		}

		public void ConnectChild(object userData)
		{
			MenuAction menuAction = userData as MenuAction;
			connectOrigin = menuAction.nodeIDs[0];
			view.StartConnectChild();
		}

		public void Delete(object userData)
		{
			MenuAction menuAction = userData as MenuAction;
			BTEditorManager.Delete(menuAction.nodeIDs.ToArray());
		}

		public void Copy(object userData)
		{
			MenuAction menuAction = userData as MenuAction;
			BTEditorManager.Copy(menuAction.nodeIDs.ToArray());
		}

		public void Paste(object userData)
		{
			MenuAction menuAction = userData as MenuAction;
			BTEditorManager.Paste(menuAction.position);
		}

		/// <summary>
		/// 保存行为树到xml文件，并生成对应lua文件
		/// </summary>
		/// <param name="userData"></param>
		public void Save(object userData)
		{
			// 序列化
			BTEditorManager.SelectAsset.Serialize(BTEditorManager.SelectTree);

			// 存储到文件
			string file = Application.dataPath.Replace("Assets", "")
				+ BTEditorManager.SelectAsset.SerializeToFile();
			BTEditorManager.ConvertFileFromXMLToLua(file);

			// 生成AI加载列表
			AIMenuEditor.GenerateAILoadList();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Debug.Log("保存结束");
		}

		/// <summary>
		/// 从同名xml文件读取行为树数据
		/// </summary>
		/// <param name="userData"></param>
		public void Load(object userData)
		{
			// BTEditorManager.BTAsset.DeserializeFromFile();
		}
	}
}