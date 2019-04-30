#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BTEditor
{
	// 不会导出的根节点，为了能让Root树和事件树在同一个节点下，为了视觉上的美观
    [TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/Root.png")]
	public class BaseRoot : BTNode
	{

		// Child connections
		[SerializeField][HideInInspector]
		List<BTNode> _children = new List<BTNode>();

		public override void ConnectChild(BTNode child)
		{
			_children.Add(child);
			child.parent = this;
		}

		public override void DisconnectChild(BTNode child)
		{
			if (_children.Contains(child))
			{
				_children.Remove(child);
				child.parent = null;
			}
			else
			{
				throw new System.InvalidOperationException(string.Format("{0} is not a child of {1}", child, this));
			}
		}

		public void SortChildren()
		{
			_children.Sort();
		}

		public override List<BTNode> Children { get { return _children; } }

		public override int ChildCount { get { return _children.Count; } }

		public override bool CanConnectChild { get { return true; } }

		public override bool ContainsChild(BTNode child) { return _children.Contains(child); }

        public override BTNode parent
		{
			get { return null; }
			set { throw new System.InvalidOperationException("The Root node cannot have a parent connection"); }
		}
		public override void Unparent()
		{
			throw new System.InvalidOperationException("The Root node cannot have a parent connection");
		}

	}
}

#endif