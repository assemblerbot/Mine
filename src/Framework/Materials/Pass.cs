using Veldrid;
using Veldrid.SPIRV;

namespace Mine.Framework;

public sealed class Pass : IDisposable
{
	public readonly  ulong                                               Id;
	public readonly  string                                              Name;
	public readonly  int                                                 Order;
	public readonly  BlendStateDescription                               BlendStateDescription;
	public readonly  DepthStencilStateDescription                        DepthStencilStateDescription;
	public readonly  RasterizerStateDescription                          RasterizerStateDescription;
	public readonly  MaterialShader                                      VertexShader;
	public readonly  MaterialShader                                      PixelShader;
	public readonly  (ShaderResourceSetKind kind, ShaderStages stages)[] ShaderResourceSets;
	public readonly  PassConstBuffer[]                                   ShaderConstBuffers;
	
	// indexation structure, const buffer index is stored in cell N for each ShaderResourceSetKind.MaterialPropertiesN 
	private readonly int[] _constBufferIndex = new int [(int) ShaderResourceSetKind.MaterialPropertiesMax - (int) ShaderResourceSetKind.MaterialPropertiesMin + 1];

	private Shader[]? _shaders;
	public  Shader[]  Shaders => _shaders ??= Engine.Graphics.Factory.CreateFromSpirv(VertexShader.CreateDescription(), PixelShader.CreateDescription());

	public ResourceLayout[] ResourceLayouts
	{
		get
		{
			ResourceLayout[] result = new ResourceLayout[ShaderResourceSets.Length];

			for (int i = 0; i < ShaderResourceSets.Length; ++i)
			{
				switch (ShaderResourceSets[i].kind)
				{
					case ShaderResourceSetKind.WorldMatrix:
						result[i] = Engine.Shared.ResourceSetLayouts.WorldMatrix;
						break;
					case ShaderResourceSetKind.ViewProjectionMatrix:
						result[i] = Engine.Shared.ResourceSetLayouts.ViewProjectionMatrix;
						break;
					case ShaderResourceSetKind.MaterialProperties0:
					case ShaderResourceSetKind.MaterialProperties1:
					case ShaderResourceSetKind.MaterialProperties2:
					case ShaderResourceSetKind.MaterialProperties3:
					case ShaderResourceSetKind.MaterialProperties4:
					case ShaderResourceSetKind.MaterialProperties5:
					case ShaderResourceSetKind.MaterialProperties6:
					case ShaderResourceSetKind.MaterialProperties7:
						result[i] = GetConstBuffer(ShaderResourceSets[i].kind).ResourceLayout;
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
		ulong                                               id,
		string                                              name,
		int                                                 order,
		BlendStateDescription                               blendStateDescription,
		DepthStencilStateDescription                        depthStencilStateDescription,
		RasterizerStateDescription                          rasterizerStateDescription,
		MaterialShader                                      vertexShader,
		MaterialShader                                      pixelShader,
		(ShaderResourceSetKind kind, ShaderStages stages)[] shaderResourceSets,
		PassConstBuffer[]                                   shaderConstBuffers
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
		ShaderResourceSets           = shaderResourceSets;
		ShaderConstBuffers           = shaderConstBuffers;

		VertexShader.Stages = ShaderStages.Vertex;
		PixelShader.Stages  = ShaderStages.Fragment;
		
		for(int i=0; i < ShaderConstBuffers.Length; ++i)
		{
			PassConstBuffer buffer = ShaderConstBuffers[i];
			
			if ((int) buffer.Kind < (int) ShaderResourceSetKind.MaterialPropertiesMin || (int) buffer.Kind > (int) ShaderResourceSetKind.MaterialPropertiesMax)
			{
				throw new InvalidOperationException($"Constant buffer `{buffer.Name}` must be an MaterialProperties buffer!");
			}

			int index = Array.FindIndex(ShaderResourceSets, item => item.kind == buffer.Kind);
			if (index == -1)
			{
				throw new InvalidOperationException($"Constant buffer `{buffer.Name}` doesn't have shader resource set declaration!");
			}
			
			buffer.Stages = ShaderResourceSets[index].stages;
			
			_constBufferIndex[(int) buffer.Kind - (int) ShaderResourceSetKind.MaterialPropertiesMin] = i;
		}
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

		foreach (PassConstBuffer buffer in ShaderConstBuffers)
		{
			buffer.Dispose();
		}
	}

	public PassConstBuffer GetConstBuffer(ShaderResourceSetKind kind)
	{
		return ShaderConstBuffers[_constBufferIndex[(int) kind - (int) ShaderResourceSetKind.MaterialPropertiesMin]];
	}
}