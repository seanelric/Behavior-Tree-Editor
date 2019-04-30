using System.IO;
using System.Xml;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BTEditor.Task;

namespace BTEditor
{
	[System.Serializable]
	public class BTAsset : ScriptableObject
	{
		#region StaticFunction

		// 通过右键菜单创建
		[MenuItem("Assets/Create/Behavior Tree", false, 80)]
		public static void CreateNewBehaviorTree(MenuCommand menuCommand)
		{
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (path == "") path = "Assets";
			else if (Path.GetExtension(path) != "")
			{
				path = path.Replace(Path.GetFileName(
					AssetDatabase.GetAssetPath(Selection.activeObject)), "");
			}

			string fullPath = AssetDatabase.GenerateUniqueAssetPath(path + "/New Behavior Tree.asset");

			BehaviorTree bt = ScriptableObject.CreateInstance<BehaviorTree>();
			// Add base root
			BaseRoot baseRoot = ScriptableObject.CreateInstance<BaseRoot>();
			baseRoot.editorPosition = new Vector2(0, 0);
			bt.SetRoot(baseRoot);
			// Add root
			Root root = ScriptableObject.CreateInstance<Root>();
			root.editorPosition = new Vector2(0, 96);
			baseRoot.ConnectChild(root);

			BTAsset btAsset = ScriptableObject.CreateInstance<BTAsset>();
			btAsset.Serialize(bt);

			AssetDatabase.CreateAsset(btAsset, fullPath);
			AssetDatabase.Refresh();
			// EditorUtility.FocusProjectWindow();
			Selection.activeObject = btAsset;
		}

		#endregion StaticFunction

		// 序列化行为树xml数据
		[SerializeField] string serializedBehaviorTree;

		public BehaviorTree BehaviorTree { get { return m_behaviorTree; } }
		BehaviorTree m_behaviorTree;

		public void Deserialize()
		{
			if (string.IsNullOrEmpty(serializedBehaviorTree))
			{
				Debug.LogError("行为树数据为空！");
				return;
			}

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(serializedBehaviorTree);

			// Behavior Tree
			BehaviorTree bt = ScriptableObject.CreateInstance<BehaviorTree>();

			// Base Root
			XmlElement baseRootEl = (XmlElement) doc.GetElementsByTagName("BaseRoot").Item(0);
			BaseRoot baseRoot;
			if (baseRootEl != null)
			{
				baseRoot = (BaseRoot)DeserializeSubTree(baseRootEl, bt);
				bt.SetRoot(baseRoot);
			}
			// 旧数据兼容处理
			else
			{
				baseRoot = ScriptableObject.CreateInstance<BaseRoot>();
				baseRoot.editorPosition = new Vector2(0, 0);
				bt.SetRoot(baseRoot);

				XmlElement rootEl = (XmlElement) doc.GetElementsByTagName("Root").Item(0);
				Root root = (Root)DeserializeSubTree(rootEl, bt);
				baseRoot.ConnectChild(root);
			}

			// Unparented nodes
			XmlElement unparentedRoot = (XmlElement) doc.GetElementsByTagName("unparented").Item(0);
			foreach (XmlNode xmlNode in unparentedRoot.ChildNodes)
			{
				XmlElement el = xmlNode as XmlElement;
				if (el != null) 
				{
					BTNode node = DeserializeSubTree(el, bt);
					// 旧数据兼容处理
					if (baseRootEl == null && baseRoot != null)
					{
						 baseRoot.ConnectChild(node);
					}
				}
			}

			m_behaviorTree = bt;
		}

		/// <summary>
		/// 记录BaseRoot和Root的ID，防止重复创建
		/// </summary>
		int baseRootID;
		int rootID;
		private BTNode DeserializeSubTree(XmlElement el, BehaviorTree bt)
		{
			BTNode node = null;
			if (el.Name == "BaseRoot")
			{
				if (!bt.nodeDic.ContainsKey(baseRootID))
				{
					node = bt.CreateNode<BaseRoot>();
					baseRootID = node.GetInstanceID();
				}
				else
				{
					return null;
				}
			}
			else if (el.Name == "Root")
			{
				if (!bt.nodeDic.ContainsKey(rootID))
				{
					node = bt.CreateNode<Root>();
					rootID = node.GetInstanceID();
				}
				else
				{
					return null;
				}
			}
			else if (!string.IsNullOrEmpty(el.Name))
			{
				string className = string.Empty;
				string script = el.GetAttribute("script");
				if (string.IsNullOrEmpty(script))
				{
					className = el.Name;
					// 原数据格式兼容处理
					if (className.Equals("ConditionVariable"))
					{
						className = "Condition";
					}
				}
				else
				{
					// 原数据格式兼容处理
					string[] names = script.Split(new char[]{ '.' });
					if (names.Length > 0)
					{
						className = names[names.Length - 1];
					}
				}

				if (BTEditorManager.NodeTypes.ContainsKey(className))
				{
					System.Type type = BTEditorManager.NodeTypes[className];
					node = bt.CreateNode(type);
				}
				else
				{
					Debug.LogError("Can't find the class type, name=" + className);
				}
			}
			else
			{
				throw new System.NotImplementedException(string.Format("{0} deserialization not implemented", el.Name));
			}

			// 解析参数数据
			if (node != null)
			{
				// 基类通用数据
				node.comment = el.GetAttribute("comment");
				string replaceShowName = el.GetAttribute("replaceShowName");
				if (!string.IsNullOrEmpty(replaceShowName))
				{
					node.replaceShowName = bool.Parse(replaceShowName);
				}

				float x = float.Parse(el.GetAttribute("editorx"));
				float y = float.Parse(el.GetAttribute("editory"));
				node.editorPosition = new Vector2(x, y);
				node.GUID = el.GetAttribute("guid");
				node.debugId = el.GetAttribute("debugId");

				// 本类数据
				// 原数据格式兼容处理
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				foreach (XmlNode paramNode in el.ChildNodes)
				{
					XmlElement paramEl = paramNode as XmlElement;
					if (paramEl != null && paramEl.Name == "param")
					{
						string key = paramEl.GetAttribute("key");
						if (!string.IsNullOrEmpty(key))
						{
							parameters[key] = paramEl.GetAttribute("value");
						}
					}
				}

				FieldInfo[] fieldInfos = node.GetType().GetFields(BindingFlags.Public
					| BindingFlags.Instance | BindingFlags.DeclaredOnly);
				foreach (var field in fieldInfos)
				{
					string value;
					if (parameters.ContainsKey(field.Name))
					{
						value = parameters[field.Name];
					}
					else
					{
						value = el.GetAttribute(field.Name);
					}

					if (!string.IsNullOrEmpty(value))
					{
						// 枚举类型特殊处理，防止数据不存在
						if (field.FieldType.BaseType == typeof(System.Enum))
						{
							if (System.Enum.IsDefined(field.FieldType, value))
							{
								field.SetValue(node, System.Enum.Parse(field.FieldType, value));
							}
						}
						else
						{
							field.SetValue(node, TypeDescriptor.GetConverter(field.FieldType).ConvertFrom(value));
						}
					}
				}

				bt.nodeDic.Add(node.GetInstanceID(), node);

				foreach (XmlNode xmlNode in el.ChildNodes)
				{
					XmlElement childEl = xmlNode as XmlElement;
					if (childEl == null || childEl.Name == "param")
					{
						continue;
					}

					BTNode child = DeserializeSubTree(childEl, bt);
					if (child != null)
					{
						node.ConnectChild(child);
					}
				}
			}

			return node;
		}

		public void Serialize(BehaviorTree bt)
		{
			BehaviorTree.AutoDebugId = 1;

			// XML Document
			XmlDocument doc = new XmlDocument();

			// Behavior Tree
			XmlElement btEl = doc.CreateElement("BehaviorTree");
			btEl.SetAttribute("name", this.name);
			doc.AppendChild(btEl);

			// Root SubTree
			SerializeSubTree(bt.rootNode, btEl);

			// Unparented nodes root
			XmlElement unparentedEl = doc.CreateElement("unparented");
			btEl.AppendChild(unparentedEl);

			// Unparented nodes
			foreach (var kvp in bt.nodeDic)
			{
				if (kvp.Value.parent == null && !(kvp.Value is BaseRoot))
				{
					SerializeSubTree(kvp.Value, unparentedEl);
				}
			}

			if (!string.IsNullOrEmpty(doc.InnerXml))
			{
				serializedBehaviorTree = doc.InnerXml;
			}
		}

		private void SerializeSubTree(BTNode node, XmlElement parentEl)
		{
			XmlDocument doc = parentEl.OwnerDocument;
			XmlElement el = doc.CreateElement(node.GetType().Name);

			// 序列化通用数据
			el.SetAttribute("comment", node.comment);
			el.SetAttribute("replaceShowName", node.replaceShowName.ToString());
			el.SetAttribute("editorx", node.editorPosition.x.ToString());
			el.SetAttribute("editory", node.editorPosition.y.ToString());
			el.SetAttribute("guid", node.GUID);
			el.SetAttribute("debugId", BehaviorTree.AutoDebugId.ToString());

			// 序列化非通用数据
			System.Type type = node.GetType();
			FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public
				| BindingFlags.Instance | BindingFlags.DeclaredOnly);
			foreach (var field in fieldInfos)
			{
				object value = field.GetValue(node);
				string realValue;
				if (value == null) realValue = string.Empty;
				else realValue = value.ToString();

				// 不需要输出的数值特殊处理
				if (string.IsNullOrEmpty(realValue))
				{
					continue;
				}

				el.SetAttribute(field.Name, realValue);
			}

			parentEl.AppendChild(el);

			int count = node.ChildCount;
			for (int i = 0; i < count; i++)
			{
				SerializeSubTree(node.Children[i], el);
			}
		}

		#region LocalFileOperation
		
		// AI编辑文件的根目录
		string AssetPathRoot = "Assets/EditTools/Data/BehaviorTree/Asset";
		// AI导出xml文件的存储根目录
		string xmlPathRoot = "Assets/EditTools/Data/BehaviorTree/Xml";

		const string AssetFolderName = "/Asset/";
		const string AssetNameExtension = ".asset";
		const string XmlFolderName = "/Xml/";
		const string XmlNameExtension = ".xml";

		public string SerializeToFile()
		{
			// Reset auto generate id
			BehaviorTree.AutoGenerateId = 1;
			BehaviorTree.AutoDebugId = 1;

			// XML Document
			XmlDocument doc = new XmlDocument();

			// Behavior Tree
			XmlElement btEl = doc.CreateElement("BehaviorTree");
			btEl.SetAttribute("name", this.name);
			doc.AppendChild(btEl);

			// Root SubTree
			for (int i = 0; i < m_behaviorTree.rootNode.ChildCount; ++i)
			{
				SerializeSubTreeToFile(m_behaviorTree.rootNode.Children[i], btEl);
			}

			// Save to xml file with same name
			return SaveXml(doc);
		}

		private void SerializeSubTreeToFile(BTNode node, XmlElement parentEl)
		{
			// 数据检查
			node.Check();

			XmlDocument doc = parentEl.OwnerDocument;
			XmlElement el = doc.CreateElement(node.GetType().Name);
			el.SetAttribute("debugId", BehaviorTree.AutoDebugId.ToString());

			System.Type type = node.GetType();
			FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public
				| BindingFlags.Instance | BindingFlags.DeclaredOnly);
			foreach (var field in fieldInfos)
			{
				object value = field.GetValue(node);
				string realValue = string.Empty;
				if (value != null)
				{
					realValue = value.ToString();

					// 转换枚举到int值
					if (field.FieldType.BaseType == typeof(System.Enum)
						&& field.FieldType != typeof(CompareType)
						&& field.FieldType != typeof(OperatorType)
						&& field.FieldType != typeof(RelativeType)
						&& field.FieldType != typeof(SuccessMode)
						&& field.FieldType != typeof(FailureMode))
					{
						realValue = ((int)value).ToString();
						// 0值枚举不输出
						if (realValue.Equals("0")) continue;
					}
				}

				// 检查需要自动生成id的参数（必须在判空前执行）
				AutoGenerateValueAttribute[] attrs = field.GetCustomAttributes(
					typeof(AutoGenerateValueAttribute), false) as AutoGenerateValueAttribute[];
				if (attrs.Length > 0)
				{
					realValue = BehaviorTree.AutoGenerateId.ToString();
				}

				// 不需要输出的数值特殊处理
				if (string.IsNullOrEmpty(realValue) || realValue.Equals("空"))
				{
					continue;
				}

				// 检查需要转换内容的参数
				string convertEnum = BehaviorTree.GetConvertEnumValue(realValue);
				if (!string.IsNullOrEmpty(convertEnum))
				{
					realValue = convertEnum;
				}

				el.SetAttribute(field.Name, realValue);
			}

			parentEl.AppendChild(el);

			// Serialize sub tree
			int count = node.ChildCount;
			for (int i = 0; i < count; i++)
			{
				SerializeSubTreeToFile(node.Children[i], el);
			}
		}

		/// <summary>
		/// Gets the asset file.
		/// </summary>
		/// <returns>The asset file.</returns>
		public string GetFullpath(){
			return  AssetDatabase.GetAssetPath(this);
		}

		// 存储xml内容
		string SaveXml(XmlDocument doc)
		{
			// 遍历存储，自动创建不存在的文件夹
			string savePath = xmlPathRoot;
			string path = AssetDatabase.GetAssetPath(this);
			string sub = path.Substring(AssetPathRoot.Length + 1);
			string[] names = sub.Split('/');

			foreach (var name in names)
			{
				savePath += "/" + name;
				// 根据后缀名判断是否为目录
				if (name.Contains(AssetNameExtension))
				{
					savePath = savePath.Replace(AssetNameExtension, XmlNameExtension);
				}
				else if (!Directory.Exists(savePath))
				{
					Directory.CreateDirectory(savePath);
				}
			}

			doc.Save(savePath);

			string[] result = sub.Split('.');
			WriteChangedFile(result[0]);
			return savePath;
		}

		string FormatXml(XmlDocument doc)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			XmlTextWriter xtw = null;
			try
			{
				xtw = new XmlTextWriter(sw);
				xtw.Formatting = Formatting.Indented;
				xtw.Indentation = 1;
				xtw.IndentChar = '\t';
				doc.WriteTo(xtw);
			}
			finally
			{
				if (xtw != null)
					xtw.Close();
			}

			return sb.ToString();
		}

		/// <summary>
		/// 存储改变文件，用于转换成lua文件
		/// </summary>
		/// <param name="content"></param>
		void WriteChangedFile(string content)
		{
			// 拼装文件路径
			string path = AssetDatabase.GetAssetPath(this);
			int end = path.IndexOf(AssetFolderName);
			path = path.Substring(0, end);
			path += "/ChangedFile.txt";

			FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            //开始写入
            sw.Write(content);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
		}

		#endregion LocalFileOperation
	}
}