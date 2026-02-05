using Microsoft.EntityFrameworkCore;
using TelAvivMuni_Exercise.Core.Contracts;

namespace TelAvivMuni_Exercise.Infrastructure;

/// <summary>
/// Entity Framework Core database context for the application.
/// Provides access to database entities and configures entity mappings.
/// </summary>
public class AppDbContext : DbContext
{
	/// <summary>
	/// Initializes a new instance of the AppDbContext.
	/// </summary>
	/// <param name="options">The options to be used by the DbContext.</param>
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	/// <summary>
	/// Gets the Products DbSet.
	/// </summary>
	public DbSet<Product> Products => Set<Product>();

	/// <inheritdoc />
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Product>(entity =>
		{
			entity.ToTable("Product", "dbo");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Id).ValueGeneratedNever();
			entity.Property(e => e.Code).HasMaxLength(20);
			entity.Property(e => e.Name).HasMaxLength(100);
			entity.Property(e => e.Category).HasMaxLength(50);
			entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
		});
	}
}
