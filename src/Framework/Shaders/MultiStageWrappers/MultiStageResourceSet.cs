using Veldrid;

namespace Mine.Framework;

public sealed class MultiStageResourceSet : IDisposable
{
	private readonly Func<ShaderStages, ResourceSetDescription>   _descriptionGetter;
	private readonly List<(ShaderStages stages, ResourceSet set)> _sets = new();

	public MultiStageResourceSet(Func<ShaderStages, ResourceSetDescription> descriptionGetter)
	{
		_descriptionGetter = descriptionGetter;
	}

	public ResourceSet Get(ShaderStages stages)
	{
		int index = -1;
		for (int i = 0; i < _sets.Count; ++i)
		{
			if (_sets[i].stages == stages)
			{
				index = i;
				break;
			}
		}

		if (index != -1)
		{
			return _sets[index].set;
		}

		ResourceSet set = Engine.Graphics.Factory.CreateResourceSet(_descriptionGetter(stages));
		_sets.Add((stages, set));
		return set;
	}

	public void Dispose()
	{
		foreach (var item in _sets)
		{
			item.set.Dispose();
		}
	}
}