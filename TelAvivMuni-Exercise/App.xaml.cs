using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelAvivMuni_Exercise.Core;
using TelAvivMuni_Exercise.Core.Contracts;
using TelAvivMuni_Exercise.Core.Contracts.Patterns;
using TelAvivMuni_Exercise.Domain;
using TelAvivMuni_Exercise.Infrastructure;
using TelAvivMuni_Exercise.Persistence;
using TelAvivMuni_Exercise.Presentation;
using TelAvivMuni_Exercise.Presentation.ViewModels;
using TelAvivMuni_Exercise.Presentation.Views;

namespace TelAvivMuni_Exercise;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
[ExcludeFromCodeCoverage]
public partial class App : Application
{
	private readonly IHost _host;

	public App()
	{
		_host = Host.CreateDefaultBuilder()
			.ConfigureServices((context, services) =>
			{
				ConfigureServices(services, context);
			})
			.Build();

		// Initialize the locator
		ViewModelLocator.Initialize(_host.Services);
	}

	private static void ConfigureServices(IServiceCollection services, HostBuilderContext context)
	{
		// Register storage provider — kind and provider are configured in appsettings.json.
		// All implementations (provider, DbContext, serializer) are discovered at runtime;
		// the App has zero compile-time knowledge of any concrete storage type or EF Core.
		services.AddStorageProvider<Product>(context.Configuration);

		services.AddSingleton<IRepository<Product>>(sp =>
			new ProductRepository(sp.GetRequiredService<IDataStore<Product>>()));
		services.AddSingleton<IUnitOfWork, UnitOfWork>();

		// Register ViewModels — pass resolved data source description for the status bar.
		var opts = context.Configuration.GetSection(StorageOptions.SectionName).Get<StorageOptions>()
				   ?? new StorageOptions();
		services.AddTransient<MainWindowViewModel>(sp =>
		{
			var store = sp.GetRequiredService<IDataStore<Product>>();
			var location = store is ILocatableDataStore loc
				? loc.Location
				: opts.ConnectionStringName ?? opts.ConnectionString ?? "—";
			var dataSourceInfo = $"{opts.Kind}  ·  {opts.Provider}  ·  {location}";
			return new MainWindowViewModel(sp.GetRequiredService<IUnitOfWork>(), dataSourceInfo);
		});

		// Register the locator as a resource
		services.AddSingleton<ViewModelLocator>();
	}

	protected override async void OnStartup(StartupEventArgs e)
	{
		// Call base.OnStartup first to ensure all Application.Resources are fully loaded
		base.OnStartup(e);

		await _host.StartAsync();
		
		// Only create MainWindow after all application resources are available
		new MainWindow().Show();
	}

	protected override async void OnExit(ExitEventArgs e)
	{
		await _host.StopAsync();
		_host.Dispose();
		base.OnExit(e);
	}
}
