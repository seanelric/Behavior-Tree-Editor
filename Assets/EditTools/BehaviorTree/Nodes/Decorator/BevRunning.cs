#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Decorator/BevRunning")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevRunning.png")]
	public class BevRunning : Decorator
	{
		public override string GetDescription()
		{
			return "返回Running";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			throw new System.NotImplementedException();
		}
	}
}

#endif