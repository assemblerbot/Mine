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
	public void SetShaderConstant(string passName, ShaderResourceSetKind kind, string elementName, Vector4Float value)
	{
		var element = (PassConstBufferVector4Float)FindElement(passName, kind, elementName);
		element.Value = value;
	}

	public void SetShaderConstant(string passName, ShaderResourceSetKind kind, string elementName, Color4FloatRGBA value)
	{
		var element = (PassConstBufferColor4FloatRgba)FindElement(passName, kind, elementName);
		element.Value = value;
	}
	
	private PassConstBufferElement FindElement(string passName, ShaderResourceSetKind kind, string elementName)
	{
		Pass? pass = FindPassByName(passName);
		if (pass is null)
		{
			throw new NullReferenceException($"Pass '{passName}' not found!");
		}

		PassConstBuffer constBuffer = pass.GetConstBuffer(kind);
		PassConstBufferElement? element = constBuffer.Elements.FirstOrDefault(x => x.Name == elementName);
		if (element is null)
		{
			throw new NullReferenceException($"Pass '{passName}' and buffer '{kind}' doesn't have element '{elementName}'!");
		}

		return element;
	}
	#endregion
}