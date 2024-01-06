using Migration;
using Mine.Studio;

namespace RedHerring.Studio.Models.Project.Importers.Definition;

[Importer(".def")]
public class DefinitionImporter : AssetImporter<DefinitionImporterSettings>
{
	protected override DefinitionImporterSettings CreateImporterSettings() => new();

	protected override ImporterResult Import(Stream stream, DefinitionImporterSettings settings, string resourcePath, MigrationManager migrationManager,
		CancellationToken                           cancellationToken)
	{
		DefinitionAsset? asset = DefinitionAsset.CreateFromStream(stream, migrationManager);
		if (asset == null)
		{
			return ImporterResult.Finished;
		}

		asset.ImportToResources(resourcePath);
		return ImporterResult.Finished;
	}
}