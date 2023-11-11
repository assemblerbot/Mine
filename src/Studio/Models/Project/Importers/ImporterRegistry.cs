using GameToolkit.Framework;

namespace RedHerring.Studio.Models.Project.Importers;

public sealed class ImporterRegistry
{
	private Dictionary<string, Importer> _importers        = new();
	private Importer                     _fallbackImporter = new CopyImporter();

	private Dictionary<Type, List<ImporterProcessor>?> _processors         = new();
	private List<ImporterProcessor>                    _fallbackProcessors = new() {new CopyImporterProcessor()};

	public ImporterRegistry()
	{
		ScanImporters();
		ScanImporterProcessors();
	}

	public Importer GetImporter(string extension)
	{
		if (_importers.TryGetValue(extension, out Importer? importer))
		{
			return importer;
		}

		return _fallbackImporter;
	}
	
	public List<ImporterProcessor> GetProcessors(Type type)
	{
		if(_processors.TryGetValue(type, out List<ImporterProcessor>? processors))
		{
			return processors!;
		}
		
		return _fallbackProcessors;
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

	private void ScanImporterProcessors()
	{
		Engine.Types.ForEachAttribute<ImporterProcessorAttribute>(
			(attribute, type) => 
			{
				ImporterProcessor importerProcessor = (ImporterProcessor) Activator.CreateInstance(type)!;
				(_processors[attribute.ProcessedType] ??= new ()).Add(importerProcessor);
			}
		);
	}
}