using Migration;
using Mine.Studio;

namespace RedHerring.Studio.Models.Project.Importers.Definition;

[Importer(".def")]
public class DefinitionImporter : AssetImporter<DefinitionImportSettings>
{
	protected override DefinitionImportSettings CreateImporterSettings() => new();

	protected override ImporterResult Import(Stream stream, DefinitionImportSettings settings, string resourcePath, MigrationManager migrationManager,
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