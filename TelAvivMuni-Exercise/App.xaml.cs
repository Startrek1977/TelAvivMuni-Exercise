using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelAvivMuni_Exercise.Core;
using TelAvivMuni_Exercise.Core.Contracts;
using TelAvivMuni_Exercise.Core.Contracts.Patterns;
using TelAvivMuni_Exercise.Infrastructure;
using TelAvivMuni_Exercise.ViewModels;

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
				ConfigureServices(services);
			})
			.Build();

		// Initialize the locator
		ViewModelLocator.Initialize(_host.Services);
	}

	private static void ConfigureServices(IServiceCollection services)
	{
		// Register infrastructure
		services.AddDbContextFactory<AppDbContext>(options =>
			options.UseSqlServer(
				@"Server=localhost\SQLEXPRESS;Database=TelAvivMuni;Integrated Security=true;TrustServerCertificate=True",
				sqlOptions => sqlOptions.EnableRetryOnFailure()));
		services.AddSingleton<IDataStore<Product>, DbDataStore<Product, AppDbContext>>();
        services.AddSingleton<IRepository<Product>>(sp => 
			new ProductRepository(sp.GetRequiredService<IDataStore<Product>>()));
		services.AddSingleton<IUnitOfWork, UnitOfWork>();

		// Register ViewModels
		services.AddTransient<MainWindowViewModel>();

		// Register the locator as a resource
		services.AddSingleton<ViewModelLocator>();
	}

	protected override async void OnStartup(StartupEventArgs e)
	{
		await _host.StartAsync();
		base.OnStartup(e);
		// StartupUri in App.xaml handles MainWindow creation
	}

	protected override async void OnExit(ExitEventArgs e)
	{
		await _host.StopAsync();
		_host.Dispose();
		base.OnExit(e);
	}
}
