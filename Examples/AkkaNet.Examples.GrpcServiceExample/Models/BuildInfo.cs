using System.ComponentModel;
using System.Reflection;

namespace AkkaNet.Examples.GrpcServiceExample.Models;


[Description("Application build information")]
public record BuildInfo
{
	private static BuildInfo? _instance;

	/// <summary>
	/// Singleton instance
	/// </summary>
	public static BuildInfo Instance => _instance ??= Create();

	/// <summary>
	/// Application version
	/// </summary>
	[Description("Application version")]
	public string? Version { get; set; }

	/// <summary>
	/// Date and time of the last build
	/// </summary>
	[Description("Date of last start")]
	public string? ApplicationStartedAtDateTime { get; set; }

	/// <summary>
	/// Indicates whether the application is running in UTC time
	/// </summary>
	[Description("Indicates whether the application is running in UTC time")]
	public bool IsUtcTime { get; set; } = true;

	private static BuildInfo Create()
	{
		if (_instance != null)
			return _instance;

		var callingAssembly = Assembly.GetCallingAssembly();
		var assemblyName = callingAssembly.GetName();
		var assemblyVersion = assemblyName.Version!;

		var date = DateTime.UtcNow;
		_instance = new BuildInfo
		{
			Version = assemblyVersion.ToString(3),
			ApplicationStartedAtDateTime = date.ToString("dd.MM.yyyy HH:mm:ss")
		};
		return _instance;
	}
}
