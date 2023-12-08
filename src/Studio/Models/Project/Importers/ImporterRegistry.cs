using Mine.Framework;

namespace RedHerring.Studio.Models.Project.Importers;

public sealed class ImporterRegistry
{
	private Dictionary<string, Importer> _importers        = new();
	private Importer                     _fallbackImporter = new CopyImporter();

	public ImporterRegistry()
	{
		ScanImporters();
	}

	public Importer GetImporter(string extension)
	{
		if (_importers.TryGetValue(extension, out Importer? importer))
		{
			return importer;
		}

		return _fallbackImporter;
	}
	
	private void ScanImporters()
	{
		Engine.Types.ForEachAttribute<ImporterAttribute>(
			(attribute, type) => 
			{
				Importer importer = (Importer) Activator.CreateInstance(type)!;
				foreach (var extension in attribute.Extensions)
				{
					_importers.Add(extension, importer);
				}
			}
		);
	}
}