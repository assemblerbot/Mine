using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceTextureReadOnly : ShaderResource
{
	public readonly AssetReference Reference;

	private Texture?     _texture;
	private TextureView? _textureView;
	
	public ShaderResourceTextureReadOnly(string name, ShaderStages stages, AssetReference reference) : base(name, ResourceKind.TextureReadOnly, stages)
	{
		Reference = reference;
	}

	public override BindableResource GetOrCreateBindableResource()
	{
		throw new NotImplementedException();
	}

	public override void             Dispose()
	{
		_textureView?.Dispose();
		_textureView = null;
		
		_texture?.Dispose();
		_texture = null;
	}

	public override void Update()
	{
	}
}