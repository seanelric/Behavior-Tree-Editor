#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    // ==================== 已弃用 ====================
	public class BevSequence : Composite
	{
        public string id;

		int lastRunning = 0;		
		bool rememberRunning = false;

		public override string GetDescription()
		{
			return "当子节点失败后退出，所有子节点成功，返回成功，否则返回Invalid，子节点顺序阻塞执行";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			// int start = rememberRunning ? lastRunning : 0;
			// for (int i = start; i < ChildCount; i++)
			// {
			// 	Node node = Children[i];
			// 	Status status = behaviorTree.Tick(node, agent, context);
			// 	if (status != Status.Success)
			// 	{
			// 		lastRunning = status == Status.Running ? i : 0;
			// 		return status;
			// 	}
			// 	else
			// 	{
			// 		lastRunning = 0;
			// 	}

			// }
			return Status.Success;
		}
	}
}

#endif