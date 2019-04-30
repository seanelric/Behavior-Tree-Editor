#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTEditor
{
	#region Attribute

	// 菜单显示名称
	[System.AttributeUsage(System.AttributeTargets.Class)]
	public class TaskCategoryAttribute : System.Attribute
	{
		// 右键菜单显示名称
		public string name;

		public TaskCategoryAttribute(string name)
		{
			this.name = name;
		}
	}

	// 行为树节点图标
	[System.AttributeUsage(System.AttributeTargets.Class)]
	public class TaskIconAttribute : System.Attribute
	{
		// 图标资源路径（使用AssetDataBase加载）
		public string name;

		public TaskIconAttribute(string name)
		{
			this.name = name;
		}
	}

	// 自动生成值的字段
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class AutoGenerateValueAttribute : System.Attribute
	{

	}

	#endregion Attribute
}

#endif