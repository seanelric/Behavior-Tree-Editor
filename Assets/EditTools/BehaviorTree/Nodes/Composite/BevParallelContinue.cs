#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Composite/BevParallelContinue")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevParallel.png")]
	public class BevParallelContinue : Composite
	{
		[AutoGenerateValue][HideInInspector]
		public string id;
		public string resetIds;
		public bool runningQuit;

		public SuccessMode successMode = SuccessMode.none;
		public FailureMode failureMode = FailureMode.none;

		public override string GetDescription()
		{
			return "并发模式，支持多种退出模式\n" + "SUCCESS_ALL：所有的子节点都成功，返回成功;\n"
				+ "SUCCESS_ANY：任意节点成功，返回成功\n" + "FAIL_ALL：所有节点都失败，返回失败\n"
				+ "FAIL_ANY：任意节点失败，返回失败";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			return Status.Success;
		}
	}
}

#endif