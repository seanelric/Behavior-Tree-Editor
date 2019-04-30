#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Decorator/BevRunningToSuccess")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevRunningToSuccess.png")]
	public class BevRunningToSuccess : Decorator
	{
		public override string GetDescription()
		{
			return "将Running转为成功，其他不变";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			throw new System.NotImplementedException();
		}
	}
}

#endif