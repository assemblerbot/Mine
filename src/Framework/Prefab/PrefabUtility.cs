namespace Mine.Framework;

public static class PrefabUtility
{
	public static Entity? Instantiate(SceneReference? sceneReference)
	{
		if (sceneReference is null)
		{
			return null;
		}

		Scene? scene = sceneReference.Value;
		if (scene is null || scene.Root is null)
		{
			return null;
		}

		return InstantiateRecursiveRoot(scene.Root, scene.Root.Translation.Point3, scene.Root.Rotation, scene.Root.Scale);
	}

	public static Entity? Instantiate(SceneReference? sceneReference, Point3Float localPosition, QuaternionFloat localRotation, Vector3Float localScale)
	{
		if (sceneReference is null)
		{
			return null;
		}

		Scene? scene = sceneReference.Value;
		if (scene is null || scene.Root is null)
		{
			return null;
		}

		return InstantiateRecursiveRoot(scene.Root, localPosition, localRotation, localScale);
	}
	
	private static Entity InstantiateRecursiveRoot(SceneNode sceneNode, Point3Float localPosition, QuaternionFloat localRotation, Vector3Float localScale)
	{
		Entity entity = new (sceneNode.Name);
		entity.LocalPosition = localPosition;
		entity.LocalRotation = localRotation;
		entity.LocalScale    = localScale;

		if (sceneNode.Children is null)
		{
			return entity;
		}

		foreach (SceneNode child in sceneNode.Children)
		{
			InstantiateRecursive(child, entity);
		}

		return entity;
	}

	private static void InstantiateRecursive(SceneNode sceneNode, Entity parent)
	{
		Entity entity = new (sceneNode.Name);
		parent.AddChild(entity);

		entity.LocalPosition = sceneNode.Translation.Point3;
		entity.LocalRotation = sceneNode.Rotation;
		entity.LocalScale    = sceneNode.Scale;

		if (sceneNode.Children is null)
		{
			return;
		}

		foreach (SceneNode child in sceneNode.Children)
		{
			InstantiateRecursive(child, entity);
		}
	}
}