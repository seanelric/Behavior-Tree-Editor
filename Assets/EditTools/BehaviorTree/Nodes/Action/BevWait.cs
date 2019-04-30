using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTEditor.Task
{
	[TaskCategory("Action/BevWait")]
	public class BevWait : Action
	{
		[Range(0, 10)]
		public float waitTime;
	}
}
