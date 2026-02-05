using Microsoft.EntityFrameworkCore;
using TelAvivMuni_Exercise.Core.Contracts;
using TelAvivMuni_Exercise.Infrastructure;
using Xunit;

namespace TelAvivMuni_Exercise.Tests.Infrastructure;

public class DbDataStoreTests : IDisposable
{
	private readonly TestDbContext _context;
	private readonly TestDbContextFactory _contextFactory;
	private readonly DbDataStore<Product, TestDbContext> _dataStore;

	public DbDataStoreTests()
	{
		var options = new DbContextOptionsBuilder<TestDbContext>()
			.UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
			.Options;

		_context = new TestDbContext(options);
		_contextFactory = new TestDbContextFactory(options);
		_dataStore = new DbDataStore<Product, TestDbContext>(_contextFactory);
	}

	public void Dispose()
	{
		_context.Database.EnsureDeleted();
		_context.Dispose();
	}

	[Fact]
	public void Constructor_ThrowsArgumentNullException_WhenContextFactoryIsNull()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new DbDataStore<Product, TestDbContext>(null!));
	}

	[Fact]
	public async Task LoadAsync_ReturnsEmptyArray_WhenTableIsEmpty()
	{
		// Act
		var result = await _dataStore.LoadAsync();

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result);
	}

	[Fact]
	public async Task LoadAsync_ReturnsEntities_WhenTableHasData()
	{
		// Arrange
		var products = new List<Product>
		{
			new() { Id = 1, Name = "Product 1", Code = "P001", Category = "Cat", Price = 10.00m, Stock = 100 },
			new() { Id = 2, Name = "Product 2", Code = "P002", Category = "Cat", Price = 20.00m, Stock = 200 }
		};
		_context.Products.AddRange(products);
		await _context.SaveChangesAsync();

		// Act
		var result = await _dataStore.LoadAsync();

		// Assert
		Assert.Equal(2, result.Length);
		Assert.Contains(result, p => p.Name == "Product 1");
		Assert.Contains(result, p => p.Name == "Product 2");
	}

	[Fact]
	public async Task SaveAsync_WritesDataToDatabase()
	{
		// Arrange
		var products = new List<Product>
		{
			new() { Id = 1, Name = "Test", Code = "T001", Category = "Cat", Price = 10.00m, Stock = 100 }
		};

		// Act
		var count = await _dataStore.SaveAsync(products);

		// Assert
		Assert.Equal(1, count);
		var savedProducts = await _context.Products.ToListAsync();
		Assert.Single(savedProducts);
		Assert.Equal("Test", savedProducts[0].Name);
	}

	[Fact]
	public async Task SaveAsync_ThrowsArgumentNullException_WhenEntitiesIsNull()
	{
		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() => _dataStore.SaveAsync(null!));
	}

	[Fact]
	public async Task SaveAsync_OverwritesExistingData()
	{
		// Arrange - Save initial data
		var initialProducts = new List<Product>
		{
			new() { Id = 1, Name = "Initial", Code = "I001", Category = "Cat", Price = 5.00m, Stock = 50 }
		};
		await _dataStore.SaveAsync(initialProducts);

		// Save new data
		var newProducts = new List<Product>
		{
			new() { Id = 2, Name = "New", Code = "N001", Category = "Cat", Price = 10.00m, Stock = 100 }
		};

		// Act
		await _dataStore.SaveAsync(newProducts);

		// Assert
		var loaded = await _dataStore.LoadAsync();
		Assert.Single(loaded);
		Assert.Equal("New", loaded[0].Name);
		Assert.Equal(2, loaded[0].Id);
	}

	[Fact]
	public async Task SaveAsync_ReturnsCorrectCount()
	{
		// Arrange
		var products = new List<Product>
		{
			new() { Id = 1, Name = "Product 1", Code = "P001", Category = "Cat", Price = 10.00m, Stock = 100 },
			new() { Id = 2, Name = "Product 2", Code = "P002", Category = "Cat", Price = 20.00m, Stock = 200 },
			new() { Id = 3, Name = "Product 3", Code = "P003", Category = "Cat", Price = 30.00m, Stock = 300 }
		};

		// Act
		var count = await _dataStore.SaveAsync(products);

		// Assert
		Assert.Equal(3, count);
	}

	[Fact]
	public async Task SaveAsync_HandlesEmptyCollection()
	{
		// Arrange - Add some initial data
		_context.Products.Add(new Product { Id = 1, Name = "Existing", Code = "E001", Category = "Cat", Price = 5.00m, Stock = 50 });
		await _context.SaveChangesAsync();

		// Act - Save empty collection
		var count = await _dataStore.SaveAsync(Array.Empty<Product>());

		// Assert
		Assert.Equal(0, count);
		var loaded = await _dataStore.LoadAsync();
		Assert.Empty(loaded);
	}

	[Fact]
	public async Task LoadAsync_PreservesAllEntityProperties()
	{
		// Arrange
		var product = new Product
		{
			Id = 42,
			Name = "Full Product",
			Code = "FP001",
			Category = "Electronics",
			Price = 999.99m,
			Stock = 50
		};
		_context.Products.Add(product);
		await _context.SaveChangesAsync();

		// Act
		var result = await _dataStore.LoadAsync();

		// Assert
		Assert.Single(result);
		var loaded = result[0];
		Assert.Equal(42, loaded.Id);
		Assert.Equal("Full Product", loaded.Name);
		Assert.Equal("FP001", loaded.Code);
		Assert.Equal("Electronics", loaded.Category);
		Assert.Equal(999.99m, loaded.Price);
		Assert.Equal(50, loaded.Stock);
	}
}

/// <summary>
/// Test DbContext for in-memory database testing.
/// </summary>
public class TestDbContext : DbContext
{
	public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
	{
	}

	public DbSet<Product> Products => Set<Product>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Product>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Id).ValueGeneratedNever();
		});
	}
}

/// <summary>
/// Test DbContext factory for creating test contexts.
/// </summary>
public class TestDbContextFactory : IDbContextFactory<TestDbContext>
{
	private readonly DbContextOptions<TestDbContext> _options;

	public TestDbContextFactory(DbContextOptions<TestDbContext> options)
	{
		_options = options;
	}

	public TestDbContext CreateDbContext()
	{
		return new TestDbContext(_options);
	}
}
