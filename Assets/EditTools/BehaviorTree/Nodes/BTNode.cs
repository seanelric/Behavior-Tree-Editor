#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEditor;

namespace BTEditor
{
    public class BTNode : ScriptableObject, System.IComparable
	{
		#region Common_Inspector
		[HideInInspector]
		public string comment;
		[HideInInspector]
		public bool replaceShowName;

		#endregion Common_Inspector

		// Editor settings
		[SerializeField]
		public Vector2 editorPosition { get; set; }

		// Used by the debugger to visually display the last status returned
		public Status? lastStatus { set; get; }
		public int lastTick { set; get; }

		// GUID
		public string GUID { set; get; }

		// Debug ID
		public string debugId { set; get; }

		// Description
		public virtual string GetDescription() { return string.Empty; }

		// Child connections
		public virtual void ConnectChild(BTNode child) { }
		public virtual void DisconnectChild(BTNode child) { }
		public virtual List<BTNode> Children { get { return null; } }
		public virtual int ChildCount { get { return 0; } }
		public virtual bool CanConnectChild { get { return false; } }
		public virtual bool ContainsChild(BTNode child) { return false; }
		public virtual void Serialize(ref XmlElement el) { }
		public virtual void Deserialize(XmlElement el) { }
		public virtual void SerializeToFile(ref XmlElement el) { }
		public virtual void DeserializeFromFile(XmlElement el) { }

		// 数据检查接口，各节点继承处理
		public virtual void Check() {}
		public virtual void DebugCheckError() {}

		// 导出数据接口，各节点继承处理
		public virtual string GetExportSkillData() { return null; }
		public virtual string GetExportBuffData() { return null; }

		// IComparable for sorting left-to-right in the visual editor
		public int CompareTo(object other)
		{
			BTNode otherNode = other as BTNode;
			return editorPosition.x < otherNode.editorPosition.x ? -1 : 1;
		}

		// Parent connections
		[SerializeField][HideInInspector]
		BTNode m_parent;
		public virtual BTNode parent
		{
			get { return m_parent; }
			set
			{
				if (value == null && m_parent != null && m_parent.ContainsChild(this))
				{
					throw new System.InvalidOperationException(string.Format("Cannot set parent of {0} to null because {1} still contains it in its children", this, value));
				}
				else if (value == null || (value != null && value.ContainsChild(this)))
				{
					m_parent = value;
				}
				else
				{
					throw new System.InvalidOperationException(string.Format("{0} must contain {1} as a child before setting the child parent property", value, this));
				}
			}
		}
		public virtual void Unparent()
		{
			if (m_parent != null)
			{
				m_parent.DisconnectChild(this);
			}
			else
			{
				Debug.LogWarning(string.Format("Attempted unparenting {0} while it has no parent"));
			}
		}

		public List<BTNode> Ancestors()
		{
			List<BTNode> ancestorNodes = new List<BTNode>();
			if (parent != null) parent.Ancestors(ref ancestorNodes);
			return ancestorNodes;
		}
		private void Ancestors(ref List<BTNode> ancestorNodes)
		{
			ancestorNodes.Add(this);
			if (parent != null) parent.Ancestors(ref ancestorNodes);
		}

		// All connections
		public virtual void Disconnect()
		{
			// Disconnect parent
			if (parent != null)
			{
				Unparent();
			}

			// Disconnect children
			if (ChildCount > 0)
			{
				for (int i = ChildCount - 1; i >= 0; i--)
				{
					DisconnectChild(Children[i]);
				}
			}
		}

		// Lifecycle
		public void OnEnable()
		{
			hideFlags = HideFlags.DontSave;
		}

		// Runtime
		public virtual Status Tick(GameObject agent, Context context) { return Status.Error; }
	}
}

#endif