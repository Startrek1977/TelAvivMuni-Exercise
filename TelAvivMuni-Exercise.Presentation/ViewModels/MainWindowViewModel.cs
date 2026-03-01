using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelAvivMuni_Exercise.Core.Contracts;
using TelAvivMuni_Exercise.Core.Contracts.Patterns;
using TelAvivMuni_Exercise.Domain;

namespace TelAvivMuni_Exercise.Presentation.ViewModels;

/// <summary>
/// ViewModel for the main application window, managing product data operations
/// through the Unit of Work pattern.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
	private readonly IUnitOfWork _unitOfWork;
	private ObservableCollection<Product> _products = new();
	private string? _errorMessage;

	public ObservableCollection<Product> Products
	{
		get => _products;
		set => SetProperty(ref _products, value);
	}

	public string? ErrorMessage
	{
		get => _errorMessage;
		set => SetProperty(ref _errorMessage, value);
	}

	private Product? _selectedProduct1;
	public Product? SelectedProduct1
	{
		get => _selectedProduct1;
		set => SetProperty(ref _selectedProduct1, value);
	}

	private ObservableCollection<Product> _selectedProducts1 = [];
	public ObservableCollection<Product> SelectedProducts1
	{
		get => _selectedProducts1;
		set => SetProperty(ref _selectedProducts1, value);
	}

	private Product? _selectedProduct2;
	public Product? SelectedProduct2
	{
		get => _selectedProduct2;
		set => SetProperty(ref _selectedProduct2, value);
	}

	[ObservableProperty]
	private string dataSourceInfo = string.Empty;

	/// <summary>
	/// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
	/// </summary>
	/// <param name="unitOfWork">The unit of work for data persistence operations.</param>
	/// <param name="dataSourceInfo">Human-readable description of the active data source for display in the status bar.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="unitOfWork"/> is null.</exception>
	public MainWindowViewModel(IUnitOfWork unitOfWork, string dataSourceInfo = "")
	{
		_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		DataSourceInfo = dataSourceInfo;
		_ = LoadProductsAsync();
	}

	/// <summary>
	/// Loads all products from the repository into the <see cref="Products"/> collection.
	/// </summary>
	/// <returns>A task representing the asynchronous load operation.</returns>
	private async Task LoadProductsAsync()
	{
		try
		{
			var products = await _unitOfWork.Products.GetAllAsync();
			Products = new ObservableCollection<Product>(products);
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Failed to load products: {ex.Message}";
		}
	}

	/// <summary>
	/// Adds a new product to the repository and saves changes.
	/// </summary>
	/// <param name="product">The product to add.</param>
	/// <returns>An <see cref="OperationResult"/> indicating success or failure.</returns>
	[RelayCommand]
	private async Task<OperationResult> AddProductAsync(Product product)
	{
		ErrorMessage = null;
		var result = await _unitOfWork.Products.AddAsync(product);
		if (!result.Success)
		{
			ErrorMessage = result.ErrorMessage;
			return result;
		}
		await _unitOfWork.SaveChangesAsync();
		Products.Add(product);
		return result;
	}

	/// <summary>
	/// Updates an existing product in the repository and saves changes.
	/// </summary>
	/// <param name="product">The product with updated values.</param>
	/// <returns>An <see cref="OperationResult"/> indicating success or failure.</returns>
	[RelayCommand]
	private async Task<OperationResult> UpdateProductAsync(Product product)
	{
		ErrorMessage = null;
		var result = await _unitOfWork.Products.UpdateAsync(product);
		if (!result.Success)
		{
			ErrorMessage = result.ErrorMessage;
			return result;
		}
		await _unitOfWork.SaveChangesAsync();
		return result;
	}

	/// <summary>
	/// Deletes a product from the repository and saves changes.
	/// </summary>
	/// <param name="product">The product to delete.</param>
	/// <returns>An <see cref="OperationResult"/> indicating success or failure.</returns>
	[RelayCommand]
	private async Task<OperationResult> DeleteProductAsync(Product product)
	{
		ErrorMessage = null;
		var result = await _unitOfWork.Products.DeleteAsync(product);
		if (!result.Success)
		{
			ErrorMessage = result.ErrorMessage;
			return result;
		}
		await _unitOfWork.SaveChangesAsync();
		Products.Remove(product);
		return result;
	}

	/// <summary>
	/// Persists all pending changes to the underlying data store.
	/// </summary>
	/// <returns>An <see cref="OperationResult"/> indicating success or failure.</returns>
	[RelayCommand]
	private async Task<OperationResult> SaveChangesAsync()
	{
		ErrorMessage = null;
		try
		{
			await _unitOfWork.SaveChangesAsync();
			return OperationResult.Ok();
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Failed to save changes: {ex.Message}";
			return OperationResult.Fail(ErrorMessage);
		}
	}
}
