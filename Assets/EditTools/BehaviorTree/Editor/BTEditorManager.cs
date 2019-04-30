using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using BTEditor.Task;

namespace BTEditor
{
	/// <summary>
	/// 用于存放和处理编辑器通用数据的管理类
	/// </summary>
	public class BTEditorManager
	{
		#region Event

		public delegate void VoidDelegate();
		public delegate void IntArrayDelegate(int[] list);

		/// <summary>
		/// 当前显示的行为树变化回调
		/// </summary>
		public static VoidDelegate onDisplayChanged;

		/// <summary>
		/// 当前界面需要重绘的回调
		/// </summary>
		public static VoidDelegate onNeedRepaint;

		/// <summary>
		/// 粘贴成功回调
		/// </summary>
		public static IntArrayDelegate onPaste;

		#endregion Event

		#region Variable

		// 节点名对应的类型列表
		public static Dictionary<string, System.Type> NodeTypes =
			new Dictionary<string, System.Type>();

		// Behavior tree datas
		public static BTAsset SelectAsset { get; private set; }
		public static BehaviorTree SelectTree { get { return SelectAsset.BehaviorTree; } }

		// 被复制的节点
		public static List<int> CopyFromNodes = new List<int>();

		// 复制时的起点坐标（用于计算粘贴时的坐标偏移量）
		public static Vector2 CopyFromPos = Vector2.zero;

		// 可创建行为树节点所在命名空间
		static string TaskNameSpace = "BTEditor.Task";

		#endregion Variable

		#region StaticFunction

		// SelectAsset management
		[MenuItem("策划工具/AI/AITester", false, 211)]
		public static void ConvertAIXmlToLua()
		{
			ConvertAllFilesFromXMLToLua ();
			//ConvertAITest();
		}

		/// <summary>
		/// 初始化行为节点类型和对应的字符串（右键菜单、序列化和反序列化使用）
		/// </summary>
		public static void RefreshNodeTypes()
		{
			BTEditorManager.NodeTypes.Clear();

			// 过滤查找所有可创建的行为树节点
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
    			{
					// Filter
					if (string.IsNullOrEmpty(type.Namespace)
						|| !type.Namespace.Contains(TaskNameSpace)
						|| !type.IsSubclassOf(typeof(BTNode)) || type.IsAbstract)
					{
						continue;
					}
					
					BTEditorManager.NodeTypes.Add(type.Name, type);
				}
			}
		}

		#endregion StaticFunction

		/// <summary>
		/// 刷新当前界面显示资源
		/// </summary>
		/// <param name="bt"></param>
		/// <param name="asset"></param>
		public static void Refresh(BTAsset asset)
		{
			SelectAsset = asset;
			asset.Deserialize();
			SelectTree.RefreshDebugID();

			if (onDisplayChanged != null)
			{
				onDisplayChanged();	
			}
		}

		public static void Dirty()
		{
			// 排序后再序列化，保证节点ID按顺序生成
			SelectTree.SortList();
			SelectTree.RefreshDebugID();
			SelectAsset.Serialize(SelectTree);
			EditorUtility.SetDirty(SelectAsset);

			if (onNeedRepaint != null)
			{
				onNeedRepaint();	
			}
		}

		/// <summary>
		/// 通过InstanceID获取节点
		/// </summary>
		public static BTNode GetNodeByID(int instanceID)
		{
			if (SelectTree != null && SelectTree.nodeDic.ContainsKey(instanceID))
			{
				return SelectTree.nodeDic[instanceID];
			}
			
			return null;
		}

		#region BehaviorTreeOperation

		public static void Add(int parentID, Vector2 position, System.Type nodeType)
		{
			BTNode node = null;
			if (nodeType != null)
			{
				node = SelectTree.CreateNode(nodeType);
			}
			else
			{
				throw new System.NotImplementedException(string.Format("Create {0} node failure", nodeType));
			}

			// GUID
			node.GUID = System.Guid.NewGuid().ToString();

			SelectTree.nodeDic.Add(node.GetInstanceID(), node);

			// Editor Position
			BTNode parent = GetNodeByID(parentID);
			if (parent != null && parent.CanConnectChild)
			{
				if (parent.ChildCount > 0)
				{
					BTNode lastSibling = parent.Children[parent.ChildCount - 1];
					node.editorPosition = lastSibling.editorPosition + new Vector2(GridRenderer.step.x * 10, 0);
				}
				else
				{
					node.editorPosition = new Vector2(parent.editorPosition.x, parent.editorPosition.y + GridRenderer.step.y * 10);
				}
				parent.ConnectChild(node);
				// SortChildren(parent);
			}
			else
			{
				float x = position.x;
				float y = position.y;
				float xOffset = x % GridRenderer.step.x;
				float yOffset = y % GridRenderer.step.y;
				node.editorPosition = new Vector2(x - xOffset, y - yOffset);
			}

			Dirty();
		}

		public static void Connect(int parentID, int childID)
		{
			BTNode parent = BTEditorManager.GetNodeByID(parentID);
			BTNode child = BTEditorManager.GetNodeByID(childID);
			if (parent != null && parent.CanConnectChild && child != null)
			{
				parent.ConnectChild(child);
				// SortChildren(parent);
				Dirty();
			}
			else
			{
				Debug.LogWarning(string.Format("{0} can't accept child {1}", parent, child));
			}
		}

		public static void Unparent(int nodeID)
		{
			BTNode node = GetNodeByID(nodeID);
			if (node == null) return;
			
			node.Unparent();
			Dirty();
		}

		public static void Delete(int[] nodeIDs)
		{
			BTNode tempNode = null;
			foreach (var nodeID in nodeIDs)
			{
				tempNode = GetNodeByID(nodeID);
				if (tempNode is BaseRoot || tempNode is Root)
				{
					EditorUtility.DisplayDialog("错误", "不可删除 BaseRoot 或 Root 节点！", "确定");
					return;
				}

				tempNode.Disconnect();
				SelectTree.nodeDic.Remove(tempNode.GetInstanceID());
				GameObject.DestroyImmediate(tempNode, true);
			}
			
			Dirty();
		}

		/// <summary>
		/// 复制节点
		/// </summary>
		/// <param name="nodeIDs"></param>
		/// <param name="fromPos"></param>
		public static void Copy(int[] nodeIDs)
		{
			BTNode tempNode = null;
			Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 max = new Vector2(float.MinValue, float.MinValue);

			foreach (var nodeID in nodeIDs)
			{
				tempNode = GetNodeByID(nodeID);
				if (tempNode is BaseRoot || tempNode is Root)
				{
					EditorUtility.DisplayDialog("错误", "不可复制 BaseRoot 或 Root 节点！", "确定");
					return;
				}

				// 计算复制节点的包围框
				if (tempNode.editorPosition.x < min.x)
				{
					min.x = tempNode.editorPosition.x;
				}
				float maxX = tempNode.editorPosition.x + NodeRenderer.Width;
				if (maxX > max.x) max.x = maxX;

				if (tempNode.editorPosition.y < min.y)
				{
					min.y = tempNode.editorPosition.y;
				}
				float maxY = tempNode.editorPosition.y + NodeRenderer.Height;
				if (maxY > max.y) max.y = maxY;
			}

			CopyFromPos.Set((min.x + max.x) / 2, (min.y + max.y) / 2);

			// 记录复制的节点id
			CopyFromNodes.Clear();
			foreach (var nodeId in nodeIDs)
			{
				CopyFromNodes.Add(nodeId);
			}
		}		

		/// <summary>
		/// 粘贴复制的节点
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="to">粘贴位置</param>
		public static void Paste(Vector2 to)
		{
			if (CopyFromNodes.Count <= 0) return;

			// <源节点id，新节点>
			var alreadyPastes = new Dictionary<int, BTNode>();
			BTNode tempNode = null;

			foreach (var nodeID in CopyFromNodes)
			{
				tempNode = GetNodeByID(nodeID);
				CopyFrom(tempNode, (to - CopyFromPos), alreadyPastes);
			}

			int count = alreadyPastes.Count;
			if (onPaste != null && count > 0)
			{
				int[] list = new int[count];
				int index = 0;
				foreach (var node in alreadyPastes.Values)
				{
					list[index] = node.GetInstanceID();
					++index;
				}

				onPaste(list);
			}

			alreadyPastes.Clear();
			Dirty();
		}

		static BTNode CopyFrom(BTNode origin, Vector2 offset, Dictionary<int, BTNode> alreadyPastes)
		{
			if (origin == null) return null;

			int originID = origin.GetInstanceID();

			// 目标节点是否已经复制完成
			BTNode node = null;
			if (alreadyPastes.ContainsKey(originID))
			{
				node = alreadyPastes[originID];
			}
			else
			{
				// System.Type nodeType = origin.GetType();
				node = DeepCopy(origin) as BTNode;

				// 坐标加上复制粘贴时鼠标位置的偏移量
				node.editorPosition += offset;
				node.GUID = System.Guid.NewGuid().ToString();

				SelectTree.nodeDic.Add(node.GetInstanceID(), node);
				alreadyPastes.Add(originID, node);
			}

			// 建立父子关系
			if (origin.parent != null)
			{
				BTNode parent = null;
				if (alreadyPastes.ContainsKey(origin.parent.GetInstanceID()))
				{
					parent = CopyFrom(origin.parent, offset, alreadyPastes);
				}

				if (parent != null && !parent.Children.Contains(node))
				{
					parent.Children.Add(node);
				}
				node.parent = parent;
			}

			return node;
		}

		/// <summary>
		/// 深拷贝
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		static object DeepCopy(object obj)
		{
			if (obj == null) return null;

			object targetDeepCopyObj;
			Type targetType = obj.GetType();

			//值类型  
			if (targetType.IsValueType == true)
			{
				targetDeepCopyObj = obj;
			}
			//引用类型   
			else
			{
				//创建引用对象
				if (obj is ScriptableObject)
				{
					targetDeepCopyObj = SelectTree.CreateNode(targetType);
				}
				else
				{
					targetDeepCopyObj = System.Activator.CreateInstance(targetType);	
				}
				
				// 拷贝成员变量
				System.Reflection.MemberInfo[] memberCollection = obj.GetType().GetMembers();
				foreach (System.Reflection.MemberInfo member in memberCollection)
				{
					//拷贝字段
					if (member.MemberType == System.Reflection.MemberTypes.Field)
					{
						System.Reflection.FieldInfo field = (System.Reflection.FieldInfo)member;
						object fieldValue = field.GetValue(obj);
						if (fieldValue is ICloneable)
						{
							field.SetValue(targetDeepCopyObj, (fieldValue as ICloneable).Clone());
						}
						else
						{
							field.SetValue(targetDeepCopyObj, DeepCopy(fieldValue));
						}

					}
					//拷贝属性
					else if (member.MemberType == System.Reflection.MemberTypes.Property)
					{
						System.Reflection.PropertyInfo myProperty = (System.Reflection.PropertyInfo)member;

						MethodInfo info = myProperty.GetSetMethod(false);
						if (info != null)
						{
							try
							{
								object propertyValue = myProperty.GetValue(obj, null);
								if (propertyValue is ICloneable)
								{
									myProperty.SetValue(targetDeepCopyObj, (propertyValue as ICloneable).Clone(), null);
								}
								else
								{
									myProperty.SetValue(targetDeepCopyObj, DeepCopy(propertyValue), null);
								}
							}
							catch (System.Exception ex) 
							{

							}
						}
					}
				}
			}

			return targetDeepCopyObj;
		}

		private const string CLINET_FILTER = "clientAI";

		/// <summary>
		/// 将所有AI.xml文件转换为lua文件
		/// </summary>
		private static void ConvertAllFilesFromXMLToLua()
		{
			string srcPath = Application.dataPath + "/EditTools/Data/BehaviorTree/Xml";

			foreach (var file in Directory.GetFiles(srcPath, "*.xml", SearchOption.AllDirectories))
			{
				ConvertFileFromXMLToLua(file);
			}
		}

		/// <summary>
		/// 转化单个xml文件为lua文件
		/// </summary>
		/// <returns>lua文件的路径</returns>
		/// <param name="xmlFile">Xml file.</param>
		public static string ConvertFileFromXMLToLua(string xmlFile)
		{
			string path = xmlFile.Replace("Xml", "Lua").Replace(".xml", ".lua");
			string text = AINode.LoadFile(xmlFile).ToString();

			File.WriteAllText(path, text);

			return path;
		}
		#endregion BehaviorTreeOperation
	}
}