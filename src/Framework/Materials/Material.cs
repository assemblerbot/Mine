using Veldrid;

namespace Mine.Framework;

public abstract class Material : IDisposable
{
	public readonly Pass[] Passes;

	public Material(params Pass[] passes)
	{
		Passes = passes;
	}

	public Pass? FindPassByName(string name)
	{
		int index = Array.FindIndex(Passes, x => x.Name == name);
		return index == -1 ? null : Passes[index];
	}

	public void Dispose()
	{
		foreach (Pass pass in Passes)
		{
			pass.Dispose();
		}
	}
	
	#region Set constants
	public void SetShaderConstant(string passName, ShaderStages stage, string elementName, Vector4Float value)
	{
		var element = (PassConstBufferVector4Float)FindElement(passName, stage, elementName);
		element.Value = value;
	}

	public void SetShaderConstant(string passName, ShaderStages stage, string elementName, Color4FloatRGBA value)
	{
		var element = (PassConstBufferColor4FloatRgba)FindElement(passName, stage, elementName);
		element.Value = value;
	}
	
	private PassConstBufferElement FindElement(string passName, ShaderStages stage, string elementName)
	{
		Pass? pass = FindPassByName(passName);
		if (pass is null)
		{
			throw new NullReferenceException($"Pass '{passName}' not found!");
		}

		PassConstBuffer? constBuffer = null;
		if ((stage & ShaderStages.Vertex) == ShaderStages.Vertex)
		{
			constBuffer = pass.VertexShaderConstBuffer;
		}
		else if ((stage & ShaderStages.Fragment) == ShaderStages.Fragment)
		{
			constBuffer = pass.PixelShaderConstBuffer;
		}

		if (constBuffer is null)
		{
			throw new NullReferenceException($"Pass '{passName}' doesn't have required const buffer for given stage!");
		}

		PassConstBufferElement? element = constBuffer.Elements.FirstOrDefault(x => x.Name == elementName);
		if (element is null)
		{
			throw new NullReferenceException($"Pass '{passName}' doesn't have element '{elementName}'!");
		}

		return element;
	}
	#endregion
}