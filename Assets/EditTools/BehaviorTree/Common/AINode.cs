using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Collections;
using System.Xml;
	
public class AINode 
{
	public int index = 1;
	private string name;
	private List<XmlNodeProperty> properties;
	private List<AINode> children;
	private AINode parent;

	public AINode()
	{
		properties = new List<XmlNodeProperty>();
		children = new List<AINode>();
	}


	#region Method
	public static AINode LoadFile(string file)
	{
		if (!File.Exists(file))
		{
			//Logger.Debug("File {0} Does not Exists!", file);
		}
		XmlDocument xmlDoc = new XmlDocument();
		try
		{
			xmlDoc.Load(file);
		}
		catch (XmlException e)
		{
			//Logger.Error("Cannot Load File {0}, Exception {1}", file, e);
		}

		if (xmlDoc != null)
		{
			AINode node = new AINode();
			node.setXmlData(xmlDoc.DocumentElement);
			return node;
		}
		return null;
	}

	public void setXmlData(XmlElement element)
	{
		this.name = element.Name;
		setAttributes(element.Attributes);
		setChildren(element.ChildNodes);
	}

	public List<AINode> findChildren(string key)
	{
		//TODO
		return null;
	}

	public string dump(string space)
	{
		StringBuilder sb = new StringBuilder();
		if(children.Count > 0)
		{
			foreach(AINode child in children)
			{
				sb.Append(string.Format("{0}[{1}]=\r\n{2}  {{\r\n",space,child.index,space,space));
				sb.Append(child.dump(space + "    "));
				sb.Append(space + "  },\r\n");
			}
		}
		sb.Append(string.Format("{0}[0]=\'{1}\',\r\n",space,name));
		foreach(XmlNodeProperty property in properties)
		{
			sb.Append(string.Format("{0}{1}=\'{2}\',\r\n", space, property.Key, property.Value));
		}
		return sb.ToString();
	}

	public override string ToString()
	{
		if (parent == null)
		{
			string btName = getNameFromProperty();
			StringBuilder sb = new StringBuilder();
			sb.Append("local ");
			sb.Append(btName);
			sb.Append(" = {\r\n");
			sb.Append(dump("  "));
			sb.Append("}\r\n");
			sb.Append("return ");
			sb.Append(btName);
			sb.Append("\r\n\r\n");
			return sb.ToString();
		} else
		{
			return dump(" ");
		}
	}
	private string getNameFromProperty()
	{
		foreach (XmlNodeProperty property in properties)
		{
			if (property.Key.Equals("name"))
			{
				return property.Value;
			}
		}
		return "";
	}
	#endregion



	private void setAttributes(XmlAttributeCollection collections)
	{
		foreach(XmlAttribute attr in collections)
		{
			properties.Add(new XmlNodeProperty(attr));
		}
	}

	private void setChildren(XmlNodeList nodelist)
	{ 
		Type xmlElementType = typeof(XmlElement);
		int i = 1;
		foreach(XmlNode node in nodelist)
		{
			if(node.GetType() == xmlElementType)
			{
				AINode child = new AINode();
				child.setXmlData((XmlElement)node);
				child.parent = this;
				this.children.Add(child);
				child.index = i++;
			} else {
				//Logger.Debug("{0} 's node {1}",this, node);
			}
		}
	}
}

class XmlNodeProperty
{
	public string Key { get; set; }
	public string Value { get; set; }

	public XmlNodeProperty()
	{
	}

	public XmlNodeProperty(XmlAttribute attr)
	{
		Key = attr.Name;
		Value = attr.Value;
		//Logger.Debug("add Property:{0} = {1}", Key, Value);
	}

	public override string ToString()
	{
		return string.Format("{0} = {1}", Key, Value);
	}
}
