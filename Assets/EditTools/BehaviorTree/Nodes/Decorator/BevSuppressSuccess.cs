#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Decorator/BevSuppressSuccess")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevSuppressSuccess.png")]
	public class BevSuppressSuccess : Decorator
	{
		public override string GetDescription()
		{
			return "将成功转为失败，其他不变";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			throw new System.NotImplementedException();
		}
	}
}

#endif