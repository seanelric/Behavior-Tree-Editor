#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using BTEditor.Task;

/*
 * Best practices for serialization:
 * - Don't use the `new` constructor
 * - Instead use ScriptableObject.CreateInstance()
 * - For initialization, use OnEnable() instead of the constructor
 * 
 * Unity calls the constructor, deserializes the data (populating the object) and THEN calls OnEnable(),
 * so the data is guaranteed to be there in this method.
 * 
 */

namespace BTEditor
{
	public enum Status
	{
		Success,
		Failure,
		Running,
		Error
	}

	public class Context
	{
		private Dictionary<string, object> context = new Dictionary<string, object>();

		public Dictionary<string, object> All
		{
			get { return context; }
		}

		public bool ContainsKey(string key)
		{
			return context.ContainsKey(key);
		}

		public T Get<T>(string key)
		{
			if (!context.ContainsKey(key))
			{
				throw new System.MissingMemberException(string.Format("Key {0} not found in the current context", key));
			}
			T value = (T) context[key];
			return value;
		}

		public T Get<T>(string key, T defaultValue)
		{
			if (!context.ContainsKey(key))
			{
				Set<T>(key, defaultValue);
				return defaultValue;
			}
			T value = (T) context[key];
			return value;
		}

		public void Set<T>(string key, T value)
		{
			context[key] = value;
		}

		public void Unset(string key)
		{
			context.Remove(key);
		}
	}

	[System.Serializable]
	public class BehaviorTree : ScriptableObject
	{
		#region UseForSave

		// 保存时需要需要转换的字符列表
		public static Dictionary<string, string> ConvertEnumValue =
			new Dictionary<string, string>()
		{
			{ "大于", ">" },
			{ "小于", "<" },
			{ "等于", "==" },
			{ "True", "true" },
			{ "False", "false" },
			{ "取余", "%" },
			{ "相等", "==" },
		};

		// 保存数据时，需要自动生成的id值，获取后自动增加
		public static int AutoGenerateId
		{
			get { return mAutoGenerateId++; }
			set { mAutoGenerateId = value; }
		}
		static int mAutoGenerateId;

		// 根据节点顺序自动生成的Debug id
		public static int AutoDebugId
		{
			get { return mAutoDebugId++; }
			set { mAutoDebugId = value; }
		}
		static int mAutoDebugId;

		#endregion UseForSave

		public BaseRoot rootNode;

		// 节点列表<intstanceID, BTNode>
		public Dictionary<int, BTNode> nodeDic = new Dictionary<int, BTNode>();

		public bool debugMode = false;
		public BTNode currentNode = null;

		public void SetRoot(BaseRoot root)
		{
			rootNode = root;
		}

		public T CreateNode<T>() where T : BTNode
		{
			T node = (T) ScriptableObject.CreateInstance<T>();
			return node;
		}

		public BTNode CreateNode(System.Type type)
		{
			BTNode node = ScriptableObject.CreateInstance(type) as BTNode;
			return node;
		}

		// Lifecycle
		public void OnEnable()
		{
			hideFlags = HideFlags.HideAndDontSave;
		}

		public void OnDestroy()
		{
			foreach (var kvp in nodeDic)
			{
				DestroyImmediate(kvp.Value);
			}
		}

		// public Status Tick(GameObject agent, Context context)
		// {
		// 	TotalTicks++;

		// 	Status result = rootNode.Tick(agent, context);
		// 	rootNode.lastStatus = result;
		// 	rootNode.lastTick = TotalTicks;
		// 	return result;
		// }

		// public Status Tick(Node node, GameObject agent, Context context)
		// {
		// 	if (nodeWillTick != null && node != currentNode)
		// 		nodeWillTick(node);

		// 	Status result = node.Tick(agent, context);

		// 	if (nodeDidTick != null)
		// 		nodeDidTick(node, result);

		// 	currentNode = node;
		// 	node.lastStatus = result;
		// 	node.lastTick = TotalTicks;
		// 	return result;
		// }

		public BTNode GetNodeByGUID(string GUID)
		{
			foreach (var kvp in nodeDic)
			{
				if (kvp.Value.GUID == GUID) return kvp.Value;
			}
			
			return null;
		}

		public void SortList()
		{
			// 排序
			foreach (var node in nodeDic.Values)
			{
				Composite composite = node as Composite;
				if (composite != null) composite.SortChildren();
			}
		}

		/// <summary>
		/// 自动生成DebugID
		/// </summary>
		public void RefreshDebugID()
		{
			AutoDebugId = 1;
			SetDebugId(rootNode);
		} 

		void SetDebugId(BTNode node)
		{
			if (!(node is BaseRoot))
			{
				node.debugId = AutoDebugId.ToString();
			}

			// 递归遍历子节点
			for (int i = 0; i < node.ChildCount; i++)
			{
				SetDebugId(node.Children[i]);
			}
		}

		// 转换枚举值为指定字符，用于和后端行为树使用值匹配
		public static string GetConvertEnumValue(string source)
		{
			string result;
			ConvertEnumValue.TryGetValue(source, out result);

			return result;
		}
	}
}

#endif