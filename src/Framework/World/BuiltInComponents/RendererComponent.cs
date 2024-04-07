using Gfx;

namespace Mine.Framework;

public abstract class RendererComponent : Component, IRenderer
{
	public int          RenderOrder { get; private set; }
	public ulong        RenderMask  { get; private set; }
	public Clipper      Clipper;
	public List<string> Passes;

	public bool            ClearColorTarget = true;
	public Color4FloatRGBA ClearColor       = Color4FloatRGBA.Black;

	public bool  ClearDepthTarget = true;
	public float ClearDepth       = 1f;

	public bool ClearStencilTarget = false;
	public byte ClearStencil       = 0;

	private CommandList? _commandList;
	private Framebuffer? _framebuffer;

	private ulong OutputId = 0; // TODO output - 0 for default frame buffer

	public RendererComponent(int renderOrder, ulong renderMask, Clipper clipper, List<string> passes)
	{
		RenderOrder = renderOrder;
		RenderMask  = renderMask;
		Clipper     = clipper;
		Passes      = passes;

		_commandList = Engine.Graphics.Factory.CreateCommandList();
	}

	public override void AfterAddedToWorld()
	{
		Engine.World.RegisterRenderer(this);
	}

	public override void BeforeRemovedFromWorld()
	{
		Engine.World.UnregisterRenderer(this);
	}

	public override void Dispose()
	{
		_commandList?.Dispose();
		_commandList = null!;

		_framebuffer?.Dispose();
		_framebuffer = null;
	}

	public virtual void Render()
	{
		if (_commandList is null)
		{
			throw new NullReferenceException("Command buffer cannot be null!");
		}

		_commandList.Begin();

		// set frame buffer
		_commandList.SetFramebuffer(Engine.Graphics.Device.SwapchainFramebuffer); // TODO output - texture

		// clear color target
		if (ClearColorTarget)
		{
			_commandList.ClearColorTarget(0, ClearColor.VeldridRgbaFloat); // TODO output - target index is always 0
		}

		// clear depth-stencil target
		if (ClearDepthTarget)
		{
			if (ClearStencilTarget)
			{
				_commandList.ClearDepthStencil(ClearDepth, ClearStencil);
			}
			else
			{
				_commandList.ClearDepthStencil(ClearDepth);
			}
		}

		// render
		RenderMeshes();
		
		// end
		_commandList.End();
		Engine.Graphics.Device.SubmitCommands(_commandList);
	}
	
	public virtual void WindowResized(Vector2Int size)
	{
	}

	public abstract ShaderResourceSetViewProjectionMatrix GetShaderResourceSetViewProjectionMatrix();
	public abstract ShaderResourceSetAmbientLight         GetShaderResourceSetAmbientLight(List<LightComponent> lights);
	public abstract ShaderResourceSetDirectionalLight     GetShaderResourceSetDirectionalLight(List<LightComponent> lights);
	
	private void RenderMeshes()
	{
		List<MeshComponent> meshes = Clipper.CollectMeshes(this);
		if (meshes.Count == 0)
		{
			return;
		}

		List<LightComponent>? lightsCache = null;
		foreach (string pass in Passes)
		{
			RenderPass(pass, meshes, ref lightsCache);
		}
	}

	private void RenderPass(string passName, List<MeshComponent> meshes, ref List<LightComponent>? lightsCache)
	{
		// collect and sort meshes by order of their render passes
		SortedList<int, (MeshComponent mesh, Pass pass)> sortedRenderObjects = new(meshes.Count);
		foreach (MeshComponent mesh in meshes)
		{
			Pass? pass = mesh.Material.FindPassByName(passName);
			if (pass == null)
			{
				continue;
			}

			sortedRenderObjects.Add(pass.Order, (mesh, pass)); // TODO - combine by material to minimize pipeline changes
		}
		
		// draw
		int              worldMatrixIndex  = -1;
		ShaderStages     worldMatrixStages = ShaderStages.Vertex;
		SharedPipelineId currentPipelineId = new();
		foreach ((MeshComponent mesh, Pass pass) renderObject in sortedRenderObjects.Values)
		{
			SharedPipelineId pipelineId = new(
				renderObject.pass.Id,
				renderObject.mesh.SharedMesh.SharedVertexLayout.Id,
				renderObject.mesh.SharedMesh.Topology,
				OutputId
			);
			
			// pipeline is different - change it
			if (pipelineId != currentPipelineId)
			{
				SharedPipeline sharedPipeline = Engine.Shared.GetOrCreatePipeline(
					pipelineId,
					renderObject.mesh.SharedMesh,
					renderObject.pass,
					Engine.Graphics.Device.SwapchainFramebuffer.OutputDescription
				); // TODO output - custom output
				
				_commandList!.SetPipeline(sharedPipeline.Pipeline);
				currentPipelineId = pipelineId;
				
				// set resources
				worldMatrixIndex = -1;
				for (int i = 0; i < renderObject.pass.ShaderResourceSets.Length; ++i)
				{
					ShaderStages stages = renderObject.pass.ShaderResourceSets[i].stages;
					switch (renderObject.pass.ShaderResourceSets[i].kind)
					{
						case ShaderResourceSetKind.WorldMatrix: // will be set later and for each mesh
							worldMatrixIndex  = i;
							worldMatrixStages = stages;
							break;
						case ShaderResourceSetKind.ViewProjectionMatrix:
							_commandList!.SetGraphicsResourceSet((uint)i, GetShaderResourceSetViewProjectionMatrix().GetResourceSet(stages));
							break;
						case ShaderResourceSetKind.MaterialProperties0:
						case ShaderResourceSetKind.MaterialProperties1:
						case ShaderResourceSetKind.MaterialProperties2:
						case ShaderResourceSetKind.MaterialProperties3:
						case ShaderResourceSetKind.MaterialProperties4:
						case ShaderResourceSetKind.MaterialProperties5:
						case ShaderResourceSetKind.MaterialProperties6:
						case ShaderResourceSetKind.MaterialProperties7:
							_commandList!.SetGraphicsResourceSet((uint)i, renderObject.pass.GetConstBuffer(renderObject.pass.ShaderResourceSets[i].kind).GetResourceSet(stages));
							break;
						case ShaderResourceSetKind.AmbientLight:
							_commandList!.SetGraphicsResourceSet((uint) i, GetShaderResourceSetAmbientLight(GetCollectedLights(ref lightsCache)).GetResourceSet(stages)); 
							break;
						case ShaderResourceSetKind.DirectionalLight:
							_commandList!.SetGraphicsResourceSet((uint) i, GetShaderResourceSetDirectionalLight(GetCollectedLights(ref lightsCache)).GetResourceSet(stages)); 
							break;
						case ShaderResourceSetKind.PointLights:
						case ShaderResourceSetKind.SpotLights:
						case ShaderResourceSetKind.Uninitialized:
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}

			// update world matrix
			if (worldMatrixIndex != -1)
			{
				_commandList!.SetGraphicsResourceSet((uint)worldMatrixIndex, renderObject.mesh.Entity.ShaderResourceWorldMatrix.GetResourceSet(worldMatrixStages));
			}

			// draw
			renderObject.mesh.Draw(_commandList!);
		}

		//lights = Clipper.CollectLights(this);
	}

	private List<LightComponent> GetCollectedLights(ref List<LightComponent>? lightsCache)
	{
		if (lightsCache is not null)
		{
			return lightsCache;
		}

		lightsCache = Clipper.CollectLights(this);
		return lightsCache;
	}
}