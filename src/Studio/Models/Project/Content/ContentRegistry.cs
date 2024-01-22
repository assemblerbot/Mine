using Mine.Framework;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

public sealed class ContentRegistry
{
	private readonly Dictionary<string, Type> _contentTypes         = new();
	private readonly Type                     _fallbackContent = typeof(ContentEmpty);

	public ContentRegistry()
	{
		ScanContents();
	}

	public Content? LoadContent(StudioModel model, ProjectNode node)
	{
		if(!_contentTypes.TryGetValue(node.Extension, out Type? type))
		{
			type = _fallbackContent;
		}

		object?  instance = Activator.CreateInstance(type);
		Content? content  = instance as Content;
		content?.Load(model, node);
		return content;
	}
	
	private void ScanContents()
	{
		Engine.Types.ForEachAttribute<ContentAttribute>(
			(attribute, type) => 
			{
				foreach (string extension in attribute.Extensions)
				{
					_contentTypes.Add(extension, type);
				}
			}
		);
	}
}