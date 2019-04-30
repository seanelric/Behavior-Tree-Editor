#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Variables/SignalVariables")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/SignalVariables.png")]
	public class SignalVariables : Composite
	{
		public override string GetDescription()
		{
			return "独立于Root的结构，信号参数列表";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			return Status.Success;
		}
	}
}

#endif