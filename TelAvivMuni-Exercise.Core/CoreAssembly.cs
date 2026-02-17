using System.Reflection;

namespace TelAvivMuni_Exercise.Core;

/// <summary>
/// Provides a reference to the Core assembly without requiring callers
/// to name or reference any concrete type within it.
/// </summary>
public static class CoreAssembly
{
	/// <summary>
	/// Returns the Core assembly. Because this method executes inside the Core project,
	/// <see cref="Assembly.GetExecutingAssembly"/> resolves to
	/// <c>TelAvivMuni-Exercise.Core.dll</c> regardless of the calling assembly.
	/// </summary>
	public static Assembly Get() => Assembly.GetExecutingAssembly();
}
