#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Decorator/BevFailureToRunning")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevFailureToRunning.png")]
	public class BevFailureToRunning : Decorator
	{
		public override string GetDescription()
		{
			return "将子节点的失败转为Running";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			throw new System.NotImplementedException();
		}
	}
}

#endif