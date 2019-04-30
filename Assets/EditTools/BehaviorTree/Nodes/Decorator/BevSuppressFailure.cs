#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Decorator/BevSuppressFailure")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevSuppressFailure.png")]
	public class BevSuppressFailure : Decorator
	{
		public override string GetDescription()
		{
			return "将失败转为成功，其他不变";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			throw new System.NotImplementedException();
		}
	}
}

#endif