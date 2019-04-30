#if UNITY_EDITOR

using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BTEditor.Task
{
	[TaskIcon("Assets/EditTools/BehaviorTree/Resources/BaseVariable.png")]
	public abstract class BaseVariable : BTNode
	{
		// Child connections
		public override void ConnectChild(BTNode child)
		{
			throw new System.InvalidOperationException(string.Format("{0} cannot have child connections", this));
		}
		public override void DisconnectChild(BTNode child)
		{
			throw new System.InvalidOperationException(string.Format("{0} cannot have child connections", this));
		}
		public override List<BTNode> Children
		{
			get
			{
				throw new System.InvalidOperationException(string.Format("{0} cannot have child connections", this));
			}
		}
		public override int ChildCount { get { return 0; } }
		public override bool CanConnectChild { get { return false; } }
		public override bool ContainsChild(BTNode child)
		{
			throw new System.InvalidOperationException(string.Format("{0} cannot have child connections", this));
		}

		// Serialization
		public override void Serialize(ref XmlElement el)
		{
			// el.SetAttribute("script", _scriptClass);
			// el.SetAttribute("scriptpath", _scriptPath);
			// el.SetAttribute("method", methodName);
			// foreach (KeyValuePair<string, ActionParameter> parameter in Parameters)
			// {
			// 	XmlElement paramEl = el.OwnerDocument.CreateElement("param");
			// 	paramEl.SetAttribute("key", parameter.Key);
			// 	paramEl.SetAttribute("type", parameter.Value.Type.ToString());
			// 	paramEl.SetAttribute("value", parameter.Value.ValueToString());
			// 	el.AppendChild(paramEl);
			// }
		}

		public override void SerializeToFile(ref XmlElement el)
		{
			// foreach (var parameter in Parameters)
			// {
			// 	string value = parameter.Value.ValueToString();
				
			// 	// 检查需要自动生成id的参数
			// 	if (BehaviorTree.AutoGenerateIdDic.ContainsKey(methodName))
			// 	{
			// 		foreach (var name in BehaviorTree.AutoGenerateIdDic[methodName])
			// 		{
			// 			if (name.Equals(parameter.Key))
			// 			{
			// 				value = BehaviorTree.AutoGenerateId.ToString();
			// 				break;
			// 			}
			// 		}
			// 	}

			// 	// 不需要输出的数值特殊处理
			// 	if (value.Equals("空") || string.IsNullOrEmpty(value))
			// 	{
			// 		continue;
			// 	}

			// 	// 检查需要转换内容的参数
			// 	string convertEnum = BehaviorTree.GetConvertEnumValue(value);
			// 	if (!string.IsNullOrEmpty(convertEnum))
			// 	{
			// 		value = convertEnum;
			// 	}

			// 	el.SetAttribute(parameter.Key, value);
			// }
		}

		// Deserialization
		public override void Deserialize(XmlElement el)
		{
			// _scriptClass = el.GetAttribute("script");
			// _scriptPath = el.GetAttribute("scriptpath");
			// methodName = el.GetAttribute("method");
			// if (methodName != null && methodInfo != null && el.HasChildNodes)
			// {
			// 	foreach (XmlNode paramNode in el.ChildNodes)
			// 	{
			// 		XmlElement paramEl = paramNode as XmlElement;
			// 		if (paramEl != null && paramEl.Name == "param")
			// 		{
			// 			string key = paramEl.GetAttribute("key");

			// 			// When a method signature changes, a serialized parameter might be gone, so we check for its existance
			// 			if (Parameters.ContainsKey(key))
			// 			{
			// 				System.Type type = System.Type.GetType(paramEl.GetAttribute("type"));
			// 				Parameters[key].Type = type;
			// 				string value = paramEl.GetAttribute("value");
			// 				if (string.IsNullOrEmpty(value))
			// 				{
			// 					Parameters[key].Value = value;
			// 				}
			// 				else
			// 				{
			// 					Parameters[key].Value = TypeDescriptor.GetConverter(type).ConvertFrom(value);	
			// 				}
			// 			}
			// 		}
			// 	}
			// }
		}

		public override void DeserializeFromFile(XmlElement el)
		{
			// foreach (var parameter in Parameters)
			// {
			// 	string value = parameter.Value.ValueToString();
			// 	string convertEnum = BehaviorTree.GetConvertEnumValue(value);
			// 	if (!string.IsNullOrEmpty(convertEnum))
			// 	{
			// 		value = convertEnum;
			// 	}

			// 	el.SetAttribute(parameter.Key, value);
			// }

			// _scriptClass = el.GetAttribute("script");
			// _scriptPath = el.GetAttribute("scriptpath");
			// methodName = el.GetAttribute("method");
			// if (methodName != null && methodInfo != null && el.HasChildNodes)
			// {
			// 	foreach (XmlNode paramNode in el.ChildNodes)
			// 	{
			// 		XmlElement paramEl = paramNode as XmlElement;
			// 		if (paramEl != null && paramEl.Name == "param")
			// 		{
			// 			string key = paramEl.GetAttribute("key");

			// 			// When a method signature changes, a serialized parameter might be gone, so we check for its existance
			// 			if (Parameters.ContainsKey(key))
			// 			{
			// 				System.Type type = System.Type.GetType(paramEl.GetAttribute("type"));
			// 				Parameters[key].Type = type;
			// 				string value = paramEl.GetAttribute("value");
			// 				Parameters[key].Value = TypeDescriptor.GetConverter(type).ConvertFrom(value);
			// 			}

			// 		}
			// 	}
			// }
		}

		// Runtime
		// private static Dictionary<string, VariableLibrary> ActionLibraries = new Dictionary<string, VariableLibrary>();
		public override Status Tick(GameObject agent, Context context)
		{
			// string actionLibID = _scriptClass + "-" + behaviorTree.GetInstanceID();

			// VariableLibrary lib;
			// if (ActionLibraries.ContainsKey(actionLibID))
			// {
			// 	lib = ActionLibraries[actionLibID];
			// }
			// else
			// {
			// 	System.Type type = System.Type.GetType(_scriptClass);
			// 	if (type == null)
			// 	{
			// 		Debug.LogWarning("An action node does not have an associated VariableLibrary");
			// 		return Status.Error;
			// 	}

			// 	lib = (VariableLibrary) System.Activator.CreateInstance(type);
			// 	lib.agent = agent;
			// 	lib.context = context;
			// 	MethodInfo actionMethod = type.GetMethod("Start");
			// 	if (actionMethod != null) actionMethod.Invoke(lib, null);
			// 	ActionLibraries[actionLibID] = (VariableLibrary) lib;
			// }

			// if (methodInfo == null)
			// {
			// 	Debug.LogWarning("An action node does not have an associated VariableLibrary method");
			// 	return Status.Error;
			// }

			// object[] parameters = new object[Parameters.Count];
			// int i = 0;
			// foreach (KeyValuePair<string, ActionParameter> parameter in Parameters)
			// {
			// 	parameters[i++] = parameter.Value.Value;
			// }

			// object result = methodInfo.Invoke(lib, parameters);

			// return (Status) result;
			return Status.Success;
		}
	}
}

#endif