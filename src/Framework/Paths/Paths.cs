namespace Mine.Framework;

public sealed class Paths
{
	// user home directory (on windows: c:\Users\USER_NAME\AppData\Roaming)
	public readonly string? HomeDirectoryAbsolutePath = 
		(Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
			? (Environment.GetEnvironmentVariable("XDG_CONFIG_HOME") ?? "~/Library/Application Support")
			: Environment.ExpandEnvironmentVariables("%APPDATA%");

	// application data directory (on windows: c:\Users\USER_NAME\AppData\Roaming\APPLICATION_NAME)
	public readonly string ApplicationDataAbsolutePath;

	// Resources
	public readonly string ResourcesRelativePath = Resources.RootRelativePath;
	public readonly string ResourcesAbsolutePath = Path.Join(Directory.GetCurrentDirectory(), Resources.RootRelativePath);
	
	// Application
	public readonly string ApplicationAbsolutePath = Directory.GetCurrentDirectory();
	
	public Paths(string applicationName)
	{
		ApplicationDataAbsolutePath = HomeDirectoryAbsolutePath is null ? Directory.GetCurrentDirectory() : Path.Join(HomeDirectoryAbsolutePath, applicationName);
	}
}