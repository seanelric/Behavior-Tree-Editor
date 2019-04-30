#if UNITY_EDITOR

using UnityEngine;

namespace BTEditor.Task
{
	// 变量参数
	[TaskCategory("Variables/Variable")]
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/Signal.png")]
	public class Variable : BaseVariable
	{
		public string name;

		public bool defaultValue;

		public BTEditor.Status _Variable()
		{
			return BTEditor.Status.Success;
		}
	}
}

#endif