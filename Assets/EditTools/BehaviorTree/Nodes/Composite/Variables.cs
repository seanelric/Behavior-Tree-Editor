#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Variables/Variables")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/Variables.png")]
	public class Variables : Composite
	{
		public override string GetDescription()
		{
			return "独立于Root的结构，参数列表";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			return Status.Success;
		}
	}
}

#endif