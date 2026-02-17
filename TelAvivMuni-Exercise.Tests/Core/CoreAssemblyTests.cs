using System.Reflection;
using TelAvivMuni_Exercise.Core;
using Xunit;

namespace TelAvivMuni_Exercise.Tests.Core;

public class CoreAssemblyTests
{
	[Fact]
	public void Get_ReturnsAssembly()
	{
		// Act
		var assembly = CoreAssembly.Get();

		// Assert
		Assert.NotNull(assembly);
	}

	[Fact]
	public void Get_ReturnsCorrectAssemblyName()
	{
		// Act
		var assembly = CoreAssembly.Get();

		// Assert
		Assert.Equal("TelAvivMuni-Exercise.Core", assembly.GetName().Name);
	}

	[Fact]
	public void Get_ReturnsSameInstanceOnMultipleCalls()
	{
		// Act
		var first = CoreAssembly.Get();
		var second = CoreAssembly.Get();

		// Assert
		Assert.Same(first, second);
	}
}
