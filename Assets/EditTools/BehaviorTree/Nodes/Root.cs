#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTEditor
{
    [TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/Root.png")]
	public class Root : BTNode
	{
		// Child connections
		[SerializeField][HideInInspector]
		BTNode m_child;

		public override void ConnectChild(BTNode child)
		{
			if (m_child == null)
			{
				m_child = child;
				child.parent = this;
			}
			else
			{
				throw new System.InvalidOperationException(string.Format("{0} already has a connected child, cannot connect {1}", this, child));
			}
		}

		public override void DisconnectChild(BTNode child)
		{
			if (m_child == child)
			{
				m_child = null;
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
				nodeList.Add(m_child);
				return nodeList;
			}
		}

		public override int ChildCount
		{
			get { return m_child != null ? 1 : 0; }
		}

		public override bool CanConnectChild
		{
			get { return m_child == null; }
		}

		public override bool ContainsChild(BTNode child)
		{
			return m_child == child;
		}
	}
}

#endif