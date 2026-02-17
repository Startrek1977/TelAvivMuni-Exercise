using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelAvivMuni_Exercise.Core;
using TelAvivMuni_Exercise.Persistence;
using TelAvivMuni_Exercise.Persistence.Database;
using TelAvivMuni_Exercise.Persistence.FileBase;

namespace TelAvivMuni_Exercise;

/// <summary>
/// Extension methods for IServiceCollection to register the storage provider
/// based on the "Storage" section in appsettings.json.
/// </summary>
public static class StorageRegistrationExtensions
{
	/// <summary>
	/// Registers IDataStore&lt;TEntity&gt; based on the "Storage" configuration section.
	/// All concrete implementations (DB provider, DbContext, file serializer) are
	/// discovered at runtime from DLLs in the application directory.
	/// No compile-time reference to any specific provider, DbContext, or EF Core
	/// type is required by the App.
	/// </summary>
	/// <typeparam name="TEntity">The entity type.</typeparam>
	/// <param name="services">The DI container.</param>
	/// <param name="configuration">The application configuration.</param>
	public static IServiceCollection AddStorageProvider<TEntity>(
		this IServiceCollection services,
		IConfiguration configuration)
		where TEntity : class, IEntity
	{
		var opts = configuration.GetSection(StorageOptions.SectionName).Get<StorageOptions>()
				   ?? new StorageOptions();

		if (opts.Kind.Equals("Database", StringComparison.OrdinalIgnoreCase))
		{
			var dbRegistrars = DiscoverRegistrars<IDbProviderRegistrar>(
				"TelAvivMuni-Exercise.Persistence.Database.*.dll");
			var ctxRegistrars = DiscoverRegistrars<IDbContextRegistrar>(
				CoreAssembly.Get());
			RegisterDatabase<TEntity>(services, configuration, opts, dbRegistrars, ctxRegistrars);
		}
		else
		{
			var fileRegistrars = DiscoverRegistrars<IFileProviderRegistrar>(
				"TelAvivMuni-Exercise.Persistence.FileBase.*.dll");
			RegisterFile<TEntity>(services, opts, fileRegistrars);
		}

		return services;
	}

	/// <summary>
	/// Scans the application base directory for DLLs matching <paramref name="dllPattern"/>
	/// and returns one instance of every concrete, default-constructible type that
	/// implements <typeparamref name="TRegistrar"/>.
	/// </summary>
	private static List<TRegistrar> DiscoverRegistrars<TRegistrar>(string dllPattern)
		where TRegistrar : class
	{
		var baseDir = AppDomain.CurrentDomain.BaseDirectory;
		var dllPaths = Directory.GetFiles(baseDir, dllPattern);
		var registrars = new List<TRegistrar>();
		var registrarType = typeof(TRegistrar);

		foreach (var dllPath in dllPaths)
		{
			Assembly assembly;
			try
			{
				assembly = Assembly.LoadFrom(dllPath);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(
					$"[StorageDiscovery] Could not load '{Path.GetFileName(dllPath)}': {ex.Message}");
				continue;
			}

			ScanAssembly(assembly, registrarType, registrars);
		}

		return registrars;
	}

	/// <summary>
	/// Scans a single already-loaded <paramref name="assembly"/> and returns one instance
	/// of every concrete, default-constructible type that implements <typeparamref name="TRegistrar"/>.
	/// </summary>
	private static List<TRegistrar> DiscoverRegistrars<TRegistrar>(Assembly assembly)
		where TRegistrar : class
	{
		var registrars = new List<TRegistrar>();
		ScanAssembly(assembly, typeof(TRegistrar), registrars);
		return registrars;
	}

	private static void ScanAssembly<TRegistrar>(Assembly assembly, Type registrarType, List<TRegistrar> registrars)
		where TRegistrar : class
	{
		Type[] exportedTypes;
		try
		{
			exportedTypes = assembly.GetExportedTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			exportedTypes = ex.Types.Where(t => t != null).ToArray()!;
		}

		foreach (var type in exportedTypes)
		{
			if (registrarType.IsAssignableFrom(type)
				&& !type.IsAbstract
				&& !type.IsInterface
				&& type.GetConstructor(Type.EmptyTypes) != null)
			{
				registrars.Add((TRegistrar)Activator.CreateInstance(type)!);
			}
		}
	}

	private static void RegisterDatabase<TEntity>(
		IServiceCollection services,
		IConfiguration config,
		StorageOptions opts,
		IEnumerable<IDbProviderRegistrar> providerRegistrars,
		IEnumerable<IDbContextRegistrar> contextRegistrars)
		where TEntity : class, IEntity
	{
		var providerList = providerRegistrars.ToList();
		var cs = ResolveConnectionString(config, opts);

		var providerRegistrar = providerList.FirstOrDefault(r =>
			r.ProviderName.Equals(opts.Provider, StringComparison.OrdinalIgnoreCase))
			?? throw new InvalidOperationException(
				$"Unknown DB provider '{opts.Provider}'. " +
				$"Discovered providers: {string.Join(", ", providerList.Select(r => r.ProviderName))}. " +
				$"Ensure the matching 'TelAvivMuni-Exercise.Persistence.Database.*.dll' is present in the application directory.");

		var ctxRegistrar = contextRegistrars.FirstOrDefault()
			?? throw new InvalidOperationException(
				$"No IDbContextRegistrar implementation found in '{CoreAssembly.Get().GetName().Name}'. " +
				"Ensure the Core assembly is present in the application directory.");

		ctxRegistrar.RegisterDbContext(services, dbOpts => providerRegistrar.Configure(dbOpts, cs));
		services.AddSingleton<IDataStore<TEntity>, DbDataStore<TEntity>>();
	}

	private static void RegisterFile<TEntity>(
		IServiceCollection services,
		StorageOptions opts,
		IEnumerable<IFileProviderRegistrar> registrars)
		where TEntity : class, IEntity
	{
		var registrarList = registrars.ToList();
		var registrar = registrarList.FirstOrDefault(r =>
			r.ProviderName.Equals(opts.Provider, StringComparison.OrdinalIgnoreCase))
			?? throw new InvalidOperationException(
				$"Unknown file provider '{opts.Provider}'. " +
				$"Discovered providers: {string.Join(", ", registrarList.Select(r => r.ProviderName))}. " +
				$"Ensure the matching 'TelAvivMuni-Exercise.Persistence.FileBase.*.dll' is present in the application directory.");

		var filePath = opts.FilePath
			?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data",
			   $"{typeof(TEntity).Name}{registrar.FileExtension}");

		services.AddSingleton<IDataStore<TEntity>>(
			_ => new FileDataStore<TEntity>(filePath, registrar.CreateSerializer<TEntity>()));
	}

	private static string ResolveConnectionString(IConfiguration config, StorageOptions opts)
	{
		if (!string.IsNullOrWhiteSpace(opts.ConnectionString))
			return opts.ConnectionString;

		if (!string.IsNullOrWhiteSpace(opts.ConnectionStringName))
		{
			var cs = config.GetConnectionString(opts.ConnectionStringName);
			if (!string.IsNullOrWhiteSpace(cs)) return cs;
		}

		throw new InvalidOperationException(
			"Storage provider requires a connection string. " +
			"Set Storage:ConnectionString or Storage:ConnectionStringName in appsettings.json.");
	}
}
