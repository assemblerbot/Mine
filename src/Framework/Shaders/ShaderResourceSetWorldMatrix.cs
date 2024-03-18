using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceSetWorldMatrix : ShaderResourceSet
{
	private          ulong  _updatedAt = 0;
	private readonly Entity _entity;
	
	public ShaderResourceSetWorldMatrix(Entity entity)
		: base(
			new ShaderResourceUniformBuffer(
				"WorldTransform", ShaderStages.Vertex,
				new ShaderConstant("WorldMatrix", ShaderConstantType.Float4x4)
			)
		)
	{
		_entity = entity;
	}

	public override void Update()
	{
		if (_entity.LocatToWorldMatrixUpdatedAt <= _updatedAt)
		{
			return; // already up-to-date
		}

		ShaderResourceUniformBuffer uniformBuffer = (ShaderResourceUniformBuffer)Resources[0];
		Engine.Graphics.Device.UpdateBuffer(uniformBuffer.GetOrCreateBuffer(), 0, _entity.LocalToWorldMatrix);
		_updatedAt = _entity.LocatToWorldMatrixUpdatedAt;
	}
}