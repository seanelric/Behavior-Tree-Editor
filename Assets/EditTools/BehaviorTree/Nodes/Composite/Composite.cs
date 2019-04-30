#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BTEditor.Task
{
	// Composite nodes ---------------------------------------------------------
	public abstract class Composite : BTNode
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

		// Runtime

		public override abstract Status Tick(GameObject agent, Context context);
	}
}

#endif