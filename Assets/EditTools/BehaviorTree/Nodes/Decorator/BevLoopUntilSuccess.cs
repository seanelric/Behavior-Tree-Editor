#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Decorator/BevLoopUntilSuccess")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevLoopUntilSuccess.png")]
	public class BevLoopUntilSuccess : Decorator
	{
		public string count;

		public override string GetDescription()
		{
			return "直到结果成功，否则一直执行子节点，可以设置最大执行次数";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			throw new System.NotImplementedException();
		}
	}
}

#endif