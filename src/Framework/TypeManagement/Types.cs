using System.Reflection;

namespace Mine.Framework;

public sealed class Types
{
	private readonly Assembly[] _assemblies = AppDomain.CurrentDomain.GetAssemblies();

	public IEnumerable<Type> AllTypes()
	{
		foreach (Assembly assembly in _assemblies)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				yield return type;
			}
		}
	}
	
	public void ForEachAttribute<T>(Action<T, Type> action) where T : Attribute
	{
		foreach (Type type in AllTypes())
		{
			IEnumerable<T> attributes = type.GetCustomAttributes<T>();
			foreach (T attribute in attributes)
			{
				action(attribute, type);
			}
		}
	}
}