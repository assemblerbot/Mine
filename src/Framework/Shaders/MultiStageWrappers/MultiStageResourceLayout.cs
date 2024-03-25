using Veldrid;

namespace Mine.Framework;

public sealed class MultiStageResourceLayout : IDisposable
{
	private readonly Func<ShaderStages, ResourceLayoutDescription>      _descriptionGetter;
	private readonly List<(ShaderStages stages, ResourceLayout layout)> _layouts = new();

	public MultiStageResourceLayout(Func<ShaderStages, ResourceLayoutDescription> descriptionGetter)
	{
		_descriptionGetter = descriptionGetter;
	}

	public ResourceLayout Get(ShaderStages stages)
	{
		int index = -1;
		for (int i = 0; i < _layouts.Count; ++i)
		{
			if (_layouts[i].stages == stages)
			{
				index = i;
				break;
			}
		}

		if (index != -1)
		{
			return _layouts[index].layout;
		}

		ResourceLayout layout = Engine.Graphics.Factory.CreateResourceLayout(_descriptionGetter(stages));
		_layouts.Add((stages, layout));
		return layout;
	}

	public void Dispose()
	{
		foreach (var item in _layouts)
		{
			item.layout.Dispose();
		}
	}
}