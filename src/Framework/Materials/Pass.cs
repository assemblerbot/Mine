using Veldrid;
using Veldrid.SPIRV;

namespace Mine.Framework;

public sealed class Pass : IDisposable
{
	public readonly ulong                        Id;
	public readonly string                       Name;
	public readonly int                          Order;
	public readonly BlendStateDescription        BlendStateDescription;
	public readonly DepthStencilStateDescription DepthStencilStateDescription;
	public readonly RasterizerStateDescription   RasterizerStateDescription;
	public readonly MaterialShader               VertexShader;
	public readonly MaterialShader               PixelShader;
	public readonly ShaderResourceSetKind[]      ShaderResourceSetsKind;

	private Shader[]? _shaders;
	public  Shader[]  Shaders => _shaders ??= Engine.Graphics.Factory.CreateFromSpirv(VertexShader.CreateDescription(), PixelShader.CreateDescription());

	public ResourceLayout[] ResourceLayouts
	{
		get
		{
			ResourceLayout[] result = new ResourceLayout[ShaderResourceSetsKind.Length];

			for (int i = 0; i < ShaderResourceSetsKind.Length; ++i)
			{
				switch (ShaderResourceSetsKind[i])
				{
					case ShaderResourceSetKind.WorldMatrix:
						result[i] = Engine.Shared.ResourceSetLayouts.WorldMatrix;
						break;
					case ShaderResourceSetKind.ViewProjectionMatrix:
						result[i] = Engine.Shared.ResourceSetLayouts.ViewProjectionMatrix;
						break;
					case ShaderResourceSetKind.MaterialProperties:
						// TODO
						break;
					case ShaderResourceSetKind.Uninitialized:
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			return result;
		}
	}
	
	public Pass(
		ulong                        id,
		string                       name,
		int                          order,
		BlendStateDescription        blendStateDescription,
		DepthStencilStateDescription depthStencilStateDescription,
		RasterizerStateDescription   rasterizerStateDescription,
		MaterialShader               vertexShader,
		MaterialShader               pixelShader,
		ShaderResourceSetKind[]      shaderResourceSetsKind
	)
	{
		Id                           = id;
		Name                         = name;
		Order                        = order;
		BlendStateDescription        = blendStateDescription;
		DepthStencilStateDescription = depthStencilStateDescription;
		RasterizerStateDescription   = rasterizerStateDescription;
		VertexShader                 = vertexShader;
		PixelShader                  = pixelShader;
		ShaderResourceSetsKind       = shaderResourceSetsKind;
	}

	public void Dispose()
	{
		if (_shaders != null)
		{
			foreach (var shader in _shaders)
			{
				shader?.Dispose();
			}

			_shaders = null;
		}
	}
}