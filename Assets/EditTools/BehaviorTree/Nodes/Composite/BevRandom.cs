#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Composite/BevRandom")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevRandom.png")]
	public class BevRandom : Composite
	{
		[AutoGenerateValue][HideInInspector]
		public string id;

		public override Status Tick(GameObject agent, Context context)
		{
			return Status.Success;
		}
	}
}

#endif