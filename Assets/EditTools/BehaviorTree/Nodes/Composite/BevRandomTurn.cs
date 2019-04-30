#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Composite/BevRandomTurn")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevRandom.png")]
	public class BevRandomTurn : Composite
	{
		[AutoGenerateValue][HideInInspector]
		public string id;
		public string delays;
		public string weights;

		public override Status Tick(GameObject agent, Context context)
		{
			return Status.Success;
		}
	}
}

#endif