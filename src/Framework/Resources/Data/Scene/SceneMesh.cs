using Veldrid;

namespace Mine.Framework;

[Serializable]
public sealed class SceneMesh
{
	public string                                   Name = "";
	public PrimitiveTopology                        Topology; 
	public BoundingBoxFloat                         BoundingBox;
	public List<Point3Float>                        Positions = null!;
	public List<Vector3Float>?                      Normals;
	public List<Vector3Float>?                      Tangents;
	public List<Vector3Float>?                      BiTangents;
	public List<SceneMeshTextureCoordinateChannel>? TextureCoordinateChannels;
	public List<SceneMeshVertexColorChannel>?       VertexColorChannels;
	
	public ushort[]? UShortIndices;
	public uint[]?   UIntIndices;

	public int VertexCount => Positions.Count;
	
	[NonSerialized] private int _vertexSize = 0;
	public  int VertexSize => _vertexSize == 0 ? _vertexSize = CalculateVertexSize() : _vertexSize;

	public byte[] BuildVertexBufferData(out VertexLayoutDescription vertexLayoutDescription)
	{
		// prepare vertex data
		byte[] vertexData = new byte[VertexCount * VertexSize];
		{
			using MemoryStream memory = new (vertexData);
			using BinaryWriter writer = new(memory);
			
			for (int i = 0; i < VertexCount; ++i)
			{
				writer.Write(Positions[i]);

				if (Normals is not null)
				{
					writer.Write(Normals[i]);
				}
				
				if (Tangents is not null)
				{
					writer.Write(Tangents[i]);
				}
				
				if (BiTangents is not null)
				{
					writer.Write(BiTangents[i]);
				}

				if (TextureCoordinateChannels is not null)
				{
					foreach (SceneMeshTextureCoordinateChannel channel in TextureCoordinateChannels)
					{
						if (channel.UV is not null)
						{
							writer.Write(channel.UV[i]);
						}

						if (channel.UVW is not null)
						{
							writer.Write(channel.UVW[i]);
						}
					}
				}

				if (VertexColorChannels is not null)
				{
					foreach (SceneMeshVertexColorChannel channel in VertexColorChannels)
					{
						writer.Write(channel.Colors[i]);
					}
				}
			}

			writer.Flush();
		}

		// prepare layout description
		{
			List<VertexElementDescription> descriptions = new();
			descriptions.Add(new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3));
			if (Normals is not null)
			{
				descriptions.Add(new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3));
			}

			if (Tangents is not null)
			{
				descriptions.Add(new VertexElementDescription("Tangent", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3));
			}

			if (BiTangents is not null)
			{
				descriptions.Add(new VertexElementDescription("BiTangent", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3));
			}

			if (TextureCoordinateChannels is not null)
			{
				for(int i=0;i <TextureCoordinateChannels.Count;++i)
				{
					SceneMeshTextureCoordinateChannel channel = TextureCoordinateChannels[i];
					if (channel.UV is not null)
					{
						descriptions.Add(new VertexElementDescription($"UV{i}", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));
					}

					if (channel.UVW is not null)
					{
						descriptions.Add(new VertexElementDescription($"UVW{i}", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3));
					}
				}
			}

			if (VertexColorChannels is not null)
			{
				for(int i=0;i <VertexColorChannels.Count;++i)
				{
					SceneMeshVertexColorChannel channel = VertexColorChannels[i];
					descriptions.Add(new VertexElementDescription($"Color{i}", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));
				}
			}
			
			vertexLayoutDescription = new VertexLayoutDescription(descriptions.ToArray());
		}

		return vertexData;
	}

	private int CalculateVertexSize()
	{
		int size = Point3Float.SizeInBytes
		           + (Normals    is not null ? Vector3Float.SizeInBytes : 0)
		           + (Tangents   is not null ? Vector3Float.SizeInBytes : 0)
		           + (BiTangents is not null ? Vector3Float.SizeInBytes : 0);

		if (TextureCoordinateChannels is not null)
		{
			foreach (SceneMeshTextureCoordinateChannel channel in TextureCoordinateChannels)
			{
				size += channel.ItemSizeInBytes;
			}
		}

		if (VertexColorChannels is not null)
		{
			size += VertexColorChannels.Count * SceneMeshVertexColorChannel.ItemSizeInBytes;
		}

		return size;
	}
}