using Migration;

namespace RedHerring.Studio.Models.Project.Importers;

public interface Importer
{
    ImportSettings CreateSettings();
    ImporterResult   Import(Stream stream, ImportSettings settings, string resourcePath, MigrationManager migrationManager, CancellationToken cancellationToken);
}