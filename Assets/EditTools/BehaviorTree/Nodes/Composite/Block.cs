#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Variables/Block")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/Block.png")]
	public class Block : Composite
	{
		public bool inter;

		public override string GetDescription()
		{
			return "独立于Root的结构，阻挡参数";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			return Status.Success;
		}
	}
}

#endif