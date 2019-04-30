#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Variables/Conditions")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/Conditions.png")]
	public class Conditions : Composite
	{
		public override string GetDescription()
		{
			return "独立于Root的结构，条件参数列表";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			return Status.Success;
		}
	}
}

#endif