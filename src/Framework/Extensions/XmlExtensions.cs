using System.Xml;

namespace Mine.Framework;

public static class XmlExtensions
{
	public static XmlNode? FindChild(this XmlNode node, string name)
	{
		return node.ChildNodes.Cast<XmlNode>().FirstOrDefault(child => child.Name == name);
	}

	public static XmlAttribute? FindAttribute(this XmlNode node, string name)
	{
		return node.Attributes?.Cast<XmlAttribute>().FirstOrDefault(child => child.Name == name);
	}

	public static string? GetAttributeValueString(this XmlNode node, string attributeName, string? defaultValue = null)
	{
		XmlAttribute? attribute = node.FindAttribute(attributeName);
		return attribute?.Value ?? defaultValue;
	}

	public static int GetAttributeValueInt(this XmlNode node, string attributeName, int defaultValue = 0)
	{
		XmlAttribute? attribute = node.FindAttribute(attributeName);
		if (attribute == null)
		{
			return defaultValue;
		}

		return int.TryParse(attribute.Value, out int value) ? value : defaultValue;
	}
}