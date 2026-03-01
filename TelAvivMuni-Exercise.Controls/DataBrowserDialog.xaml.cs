using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TelAvivMuni_Exercise.Core.Contracts;
using TelAvivMuni_Exercise.Core.Contracts.ViewModels;
using TelAvivMuni_Exercise.Domain;

namespace TelAvivMuni_Exercise.Controls;

/// <summary>
/// Dialog window for browsing and selecting items from a collection with filtering support.
/// Uses View-First initialization: the dialog is created and rendered first, then
/// data is loaded via ViewModel.Initialize() in the ContentRendered event.
/// </summary>
[ExcludeFromCodeCoverage]
public partial class DataBrowserDialog : Window
{
	// Flag to prevent re-entrant selection sync between DataGrid and ViewModel
	private bool _isSyncingSelection;

	public DataBrowserDialog()
	{
		InitializeComponent();

		// After Initialize() populates ViewModel.SelectedItems, sync back to the DataGrid
		DataContextChanged += OnDataContextChanged;
	}

	private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		// Detach from the previous DataContext, if it was an IMultiSelectViewModel
		if (e.OldValue is IMultiSelectViewModel oldVm)
		DataContextChanged += OnDataContextChanged;

		// Ensure we detach from the current ViewModel when the window is closed
		Closed += OnClosed;
	}

	private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		// Unsubscribe from the previous DataContext's SelectedItems, if applicable
		if (e.OldValue is IMultiSelectViewModel oldVm && oldVm.AllowMultipleSelection)
		{
			oldVm.SelectedItems.CollectionChanged -= OnViewModelSelectedItemsChanged;
		}

		// Subscribe to the new DataContext's SelectedItems, if applicable
		if (e.NewValue is IMultiSelectViewModel vm && vm.AllowMultipleSelection)
		{
			vm.SelectedItems.CollectionChanged += OnViewModelSelectedItemsChanged;
		}
	}

	private void OnClosed(object? sender, System.EventArgs e)
	{
		// Detach from the current DataContext when the window is closed
		if (DataContext is IMultiSelectViewModel vm && vm.AllowMultipleSelection)
		{
			vm.SelectedItems.CollectionChanged -= OnViewModelSelectedItemsChanged;
		}
	}
	/// <summary>
	/// Syncs the ViewModel's SelectedItems back into the DataGrid.
	/// Handles Add actions (items appended by Initialize()) and Reset/Remove actions
	/// (collection cleared externally) by doing a full re-sync of the DataGrid selection.
	/// </summary>
	private void OnViewModelSelectedItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (_isSyncingSelection) return;
		_isSyncingSelection = true;
		try
		{
			if (DataContext is not IMultiSelectViewModel vm)
				return;

			// Guard: SelectedItems can only be mutated in Extended/Multiple mode.
			// The DataTrigger that switches SelectionMode may not have applied yet when
			// Initialize() first populates the VM collection (DataContext is set before
			// the window is rendered), so check the actual mode before touching it.
			if (ProductsDataGrid.SelectionMode == DataGridSelectionMode.Single)
				return;

			// Full re-sync: rebuild DataGrid selection from ViewModel's collection
			ProductsDataGrid.SelectedItems.Clear();
			foreach (var item in vm.SelectedItems)
				ProductsDataGrid.SelectedItems.Add(item);
		}
		finally
		{
			_isSyncingSelection = false;
		}
	}

	/// <summary>
	/// Handles DataGrid selection changes in multi-select mode.
	/// WPF DataGrid.SelectedItems has no bindable setter, so this code-behind handler
	/// pushes the DataGrid selection into the ViewModel's SelectedItems collection.
	/// </summary>
	private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (_isSyncingSelection) return;
		if (DataContext is not IMultiSelectViewModel vm || !vm.AllowMultipleSelection)
			return;

		_isSyncingSelection = true;
		try
		{
			vm.SelectedItems.Clear();
			foreach (var item in ProductsDataGrid.SelectedItems)
				vm.SelectedItems.Add(item);
		}
		finally
		{
			_isSyncingSelection = false;
		}
	}

	/// <summary>
	/// Handles the AutoGeneratingColumn event to apply custom column configurations.
	/// This event is fired for each column as the DataGrid auto-generates columns from the data source.
	/// Custom columns can specify header text, width, format, and horizontal alignment.
	/// Columns not in the custom configuration are hidden.
	/// </summary>
	private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
	{
		// Only apply custom column configuration if columns are defined in the ViewModel
		if (DataContext is not IColumnConfiguration columnConfig || !columnConfig.HasCustomColumns)
			return;

		// Find the custom column configuration for this property
		var customColumn = columnConfig.Columns?.FirstOrDefault(c => c.DataField == e.PropertyName);

		if (customColumn == null)
		{
			// Hide columns that are not in the custom configuration
			e.Cancel = true;
			return;
		}

		// Set custom header text
		e.Column.Header = customColumn.Header;

		// Set custom width if specified
		if (!double.IsNaN(customColumn.Width))
		{
			e.Column.Width = new DataGridLength(customColumn.Width);
		}

		// Apply formatting and alignment for text columns
		if (e.Column is DataGridTextColumn textColumn)
		{
			// Apply format string if specified (e.g., currency, date formats)
			if (!string.IsNullOrEmpty(customColumn.Format))
			{
				textColumn.Binding = new Binding(e.PropertyName)
				{
					StringFormat = customColumn.Format
				};
			}

			// Apply horizontal alignment if specified
			if (!string.IsNullOrEmpty(customColumn.HorizontalAlignment))
			{
				var style = new Style(typeof(TextBlock));
				if (customColumn.HorizontalAlignment.Equals("Right", StringComparison.OrdinalIgnoreCase))
				{
					style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right));
				}
				else if (customColumn.HorizontalAlignment.Equals("Center", StringComparison.OrdinalIgnoreCase))
				{
					style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));
				}
				textColumn.ElementStyle = style;
			}
		}
	}
}
