using Veldrid;

namespace Mine.Framework;

internal sealed class SharedResourceSetLayoutCombined : IDisposable
{
	private readonly ResourceLayoutDescription                          _description;
	private readonly List<(ShaderStages stages, ResourceLayout layout)> _layouts = new();

	public SharedResourceSetLayoutCombined(ResourceLayoutDescription description)
	{
		_description = description;
	}

	public ResourceLayout GetResourceLayout(ShaderStages stages)
	{
		int index = _layouts.FindIndex(x => x.stages == stages);
		if (index == -1)
		{
			ResourceLayout layout = Engine.Graphics.Factory.CreateResourceLayout(_description);
			_layouts.Add((stages, layout));
			return layout;
		}

		return _layouts[index].layout;
	}

	public void Dispose()
	{
		foreach (var item in _layouts)
		{
			item.layout.Dispose();
		}
	}
}