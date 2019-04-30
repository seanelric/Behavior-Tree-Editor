#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Decorator/BevSuccess")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevSuccess.png")]
	public class BevSuccess : Decorator
	{
		public override string GetDescription()
		{
			return "返回成功";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			throw new System.NotImplementedException();
		}
	}
}

#endif