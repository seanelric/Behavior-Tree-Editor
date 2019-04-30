#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Composite/BevDetector")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevDetector.png")]
	public class BevDetector : Composite
	{
		int lastRunning = 0;
		bool rememberRunning = false;

		public override string GetDescription()
		{
			return "当子节点执行不成功后退出，返回执行结果，否则返回Invalid，子节点顺序不阻塞执行";
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
			// }
			return Status.Error;
		}
	}
}

#endif