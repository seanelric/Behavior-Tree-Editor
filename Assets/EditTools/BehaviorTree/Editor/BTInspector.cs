using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using BTEditor.Task;

namespace BTEditor
{
    [CustomEditor(typeof(HivemindInspector), true)]
	public class HivemindInspector : Editor
	{
		private static GUIStyle _titleStyle;
		private static GUIStyle _subtitleStyle;
		private static GUIStyle _helpBoxStyle;

		public static GUIStyle TitleStyle
		{
			get
			{
				if (_titleStyle == null)
				{
					_titleStyle = new GUIStyle();
					_titleStyle.fontSize = 18;
				}
				return _titleStyle;
			}
		}

		public static GUIStyle SubtitleStyle
		{
			get
			{
				if (_subtitleStyle == null)
				{
					_subtitleStyle = new GUIStyle();
					_subtitleStyle.fontSize = 15;
				}
				return _subtitleStyle;
			}
		}

		public static GUIStyle HelpBoxStyle
		{
			get
			{
				if (_helpBoxStyle == null)
				{
					_helpBoxStyle = new GUIStyle(EditorStyles.helpBox);
					_helpBoxStyle.fontSize = 12;
				}

				return _helpBoxStyle;
			}
		}
	}
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BTAsset), true)]
	public class BTInspector : HivemindInspector
	{
		void OnEnable()
		{
			BTAsset btAsset = serializedObject.targetObject as BTAsset;
			BTEditorManager.RefreshNodeTypes();
			BTEditorManager.Refresh(btAsset);
		}

		public override void OnInspectorGUI()
		{
			if (BTEditorManager.SelectTree != null)
			{
				if (BTEditorManager.SelectTree.nodeDic.Count > 2)
				{
					EditorGUILayout.LabelField(string.Format("{0} nodes",
						BTEditorManager.SelectTree.nodeDic.Count - 1), TitleStyle);
				}
				else if (BTEditorManager.SelectTree.nodeDic.Count == 2)
				{
					EditorGUILayout.LabelField("Empty", TitleStyle);
				}
				else EditorGUILayout.LabelField("1 node", TitleStyle);
			}

			EditorGUILayout.Space();

			if (GUILayout.Button("Show Behavior Tree editor"))
			{
				BTEditorWindow.ShowWindow();
			}

			if (GUI.changed) BTEditorManager.Dirty();
		}
	}
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BTNode), true)]
	public class BTNodeInspector : HivemindInspector
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			BTNode node = serializedObject.targetObject as BTNode;
			DrawInspector(node);

			base.DrawDefaultInspector();

			if (GUI.changed) BTEditorManager.Dirty();

			serializedObject.ApplyModifiedProperties();
		}

		protected override void OnHeaderGUI()
		{
			base.OnHeaderGUI();

			BTNode node = serializedObject.targetObject as BTNode;
			target.name = node.GetType().Name;
		}

		/// <summary>
		/// Draw common field
		/// </summary>
		/// <param name="node"></param>
		public void DrawInspector(BTNode node)
		{
			// 绘制各节点的说明文本
			string description = node.GetDescription();
			if (!string.IsNullOrEmpty(description))
			{
				EditorGUILayout.LabelField(description, HelpBoxStyle);
				EditorGUILayout.Separator();
			}

			EditorGUILayout.LabelField(new GUIContent("自定义显示名称"), TitleStyle);
			EditorGUILayout.Space();

			node.replaceShowName = EditorGUILayout.Toggle("Replace show name", node.replaceShowName);

			EditorGUI.BeginDisabledGroup(node.replaceShowName == false);
			node.comment = EditorGUILayout.TextField("name", node.comment);
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Separator();

			EditorGUILayout.LabelField(new GUIContent("参数"), TitleStyle);
			EditorGUILayout.Space();
		}
	}
}