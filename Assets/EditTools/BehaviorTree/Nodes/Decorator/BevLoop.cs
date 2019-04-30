#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
    [TaskCategory("Decorator/BevLoop")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Nodes/BevLoop.png")]
	public class BevLoop : Decorator
	{
		public string count;

		public override string GetDescription()
		{
			return "循环执行子节点，失败退出，可以设置成功执行次数，次数满足后退出并返回成功";
		}

		public override Status Tick(GameObject agent, Context context)
		{
			throw new System.NotImplementedException();
		}
	}
}

#endif