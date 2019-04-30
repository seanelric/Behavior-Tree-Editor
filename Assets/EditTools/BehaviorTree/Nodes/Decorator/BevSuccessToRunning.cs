#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Decorator/BevSuccessToRunning")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevSuccessToRunning.png")]
	public class BevSuccessToRunning : Decorator
	{
		public override string GetDescription()
		{
			return "将成功转为Running，其他不变";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			throw new System.NotImplementedException();
		}
	}
}

#endif