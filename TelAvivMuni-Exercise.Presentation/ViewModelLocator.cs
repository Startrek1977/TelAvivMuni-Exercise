using Microsoft.Extensions.DependencyInjection;
using TelAvivMuni_Exercise.Presentation.ViewModels;

namespace TelAvivMuni_Exercise.Presentation;

/// <summary>
/// Provides ViewModel resolution for Views without direct type coupling.
/// </summary>
public class ViewModelLocator
{
	private static IServiceProvider? _serviceProvider;

	/// <summary>
	/// Initializes the locator with the application's service provider.
	/// Must be called during application startup before resolving ViewModels.
	/// </summary>
	/// <param name="serviceProvider">The dependency injection service provider.</param>
	public static void Initialize(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	/// <summary>
	/// Resolves a ViewModel instance by its type.
	/// </summary>
	/// <param name="viewModelType">The type of ViewModel to resolve.</param>
	/// <returns>The resolved ViewModel instance.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the locator is not initialized.</exception>
	public static object Resolve(Type viewModelType)
	{
		return _serviceProvider?.GetRequiredService(viewModelType)
			?? throw new InvalidOperationException("ViewModelLocator not initialized");
	}

	/// <summary>
	/// Resolves a ViewModel instance by its generic type parameter.
	/// </summary>
	/// <typeparam name="TViewModel">The type of ViewModel to resolve.</typeparam>
	/// <returns>The resolved ViewModel instance.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the locator is not initialized.</exception>
	public static TViewModel Resolve<TViewModel>() where TViewModel : class
	{
		return _serviceProvider?.GetRequiredService<TViewModel>()
			?? throw new InvalidOperationException("ViewModelLocator not initialized");
	}

	/// <summary>
	/// Gets the MainWindowViewModel instance for XAML data binding.
	/// </summary>
	public MainWindowViewModel MainWindow
		=> Resolve<MainWindowViewModel>();
}