#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BTEditor.Task
{
	public abstract class Decorator : BTNode
	{
		// Child connections
		[SerializeField][HideInInspector]
		BTNode _child;

		public override void ConnectChild(BTNode child)
		{
			if (_child == null)
			{
				_child = child;
				child.parent = this;
			}
			else
			{
				throw new System.InvalidOperationException(string.Format("{0} already has a connected child, cannot connect {1}", this, child));
			}
		}

		public override void DisconnectChild(BTNode child)
		{
			if (_child == child)
			{
				_child = null;
				child.parent = null;
			}
			else
			{
				throw new System.InvalidOperationException(string.Format("{0} is not a child of {1}", child, this));
			}
		}

		public override List<BTNode> Children
		{
			get
			{
				List<BTNode> nodeList = new List<BTNode>();
				nodeList.Add(_child);
				return nodeList;
			}
		}

		public override int ChildCount
		{
			get { return _child != null ? 1 : 0; }
		}

		public override bool CanConnectChild
		{
			get { return _child == null; }
		}

		public override bool ContainsChild(BTNode child)
		{
			return _child == child;
		}

		// Runtime

		public override Status Tick(GameObject agent, Context context)
		{
			throw new System.NotImplementedException();
		}
	}
}

#endif