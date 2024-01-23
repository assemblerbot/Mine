﻿using Migration;

namespace RedHerring.Studio.Models.Project.Importers;

// fallback importer if no other importer is found
[Importer]
public sealed class CopyImporter : AssetImporter<CopyImportSettings>
{
	protected override CopyImportSettings CreateImporterSettings() => new();

	protected override ImporterResult Import(Stream stream, CopyImportSettings settings, string resourcePath, MigrationManager migrationManager,
		CancellationToken                           cancellationToken)
	{
		byte[] data = new byte[stream.Length];
		int read   = stream.Read(data, 0, data.Length);
		// TODO report error if read != result.Length?
		
		Directory.CreateDirectory(Path.GetDirectoryName(resourcePath)!);
		File.WriteAllBytes(resourcePath, data);
		return ImporterResult.Finished;
	}
}