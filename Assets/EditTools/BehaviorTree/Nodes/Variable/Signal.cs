#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
	// 信号参数
	[TaskCategory("Variables/Signal")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Signal.png")]
	public class Signal : BaseVariable
	{
		// 信号名称
		public string name;

		// 信号值
		public bool value;

		// 信号变量
		public string variable;
	}
}

#endif