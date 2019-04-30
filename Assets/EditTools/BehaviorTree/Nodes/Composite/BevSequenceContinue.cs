#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Composite/BevSequenceContinue")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevSequence.png")]
	public class BevSequenceContinue : Composite
	{
		[AutoGenerateValue][HideInInspector]
        public string id;
		
        int lastRunning = 0;
		bool rememberRunning = false;

		public override string GetDescription()
		{
			return "当子节点失败后退出，所有子节点成功，返回成功，否则返回Invalid，子节点顺序阻塞执行";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			return Status.Success;
		}
	}
}

#endif