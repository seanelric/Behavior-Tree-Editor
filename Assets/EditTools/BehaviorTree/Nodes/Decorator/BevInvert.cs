#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Decorator/BevInvert")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevInvert.png")]
	public class BevInvert : Decorator
	{
		public override string GetDescription()
		{
			return "将子节点的执行结果取反";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			throw new System.NotImplementedException();
		}
	}
}

#endif