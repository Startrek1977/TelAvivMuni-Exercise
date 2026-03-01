using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace TelAvivMuni_Exercise.Controls.Behaviors;

/// <summary>
/// Attached behavior that performs two-way synchronization between a DataGrid's
/// multi-selection and a bound <see cref="ObservableCollection{T}"/> on the ViewModel.
/// This keeps selection-sync logic out of the view code-behind.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DataGridMultiSelectBehavior
{
	// Per-DataGrid state (handler references + re-entrancy guard)
	private static readonly ConditionalWeakTable<DataGrid, SyncState> _states = new();

	/// <summary>
	/// The collection to keep in sync with the DataGrid's selected items.
	/// Bind this to the ViewModel's SelectedItems property.
	/// </summary>
	public static readonly DependencyProperty SelectedItemsProperty =
		DependencyProperty.RegisterAttached(
			"SelectedItems",
			typeof(ObservableCollection<object>),
			typeof(DataGridMultiSelectBehavior),
			new PropertyMetadata(null, OnSelectedItemsChanged));

	public static ObservableCollection<object>? GetSelectedItems(DependencyObject obj) =>
		(ObservableCollection<object>?)obj.GetValue(SelectedItemsProperty);

	public static void SetSelectedItems(DependencyObject obj, ObservableCollection<object> value) =>
		obj.SetValue(SelectedItemsProperty, value);

	private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not DataGrid dataGrid)
			return;

		// Detach from previous collection
		if (e.OldValue is ObservableCollection<object> oldCollection && _states.TryGetValue(dataGrid, out var oldState))
		{
			oldCollection.CollectionChanged -= oldState.CollectionChangedHandler;
			dataGrid.SelectionChanged -= oldState.DataGridSelectionChangedHandler;
			_states.Remove(dataGrid);
		}

		// Attach to new collection
		if (e.NewValue is ObservableCollection<object> newCollection)
		{
			var state = new SyncState();
			state.CollectionChangedHandler = (_, _) => OnCollectionChanged(dataGrid, newCollection, state);
			state.DataGridSelectionChangedHandler = (_, _) => OnDataGridSelectionChanged(dataGrid, newCollection, state);
			_states.Add(dataGrid, state);

			dataGrid.SelectionChanged += state.DataGridSelectionChangedHandler;
			newCollection.CollectionChanged += state.CollectionChangedHandler;
		}
	}

	/// <summary>
	/// Pushes DataGrid selection changes into the bound collection.
	/// </summary>
	private static void OnDataGridSelectionChanged(DataGrid dataGrid, ObservableCollection<object> selectedItems, SyncState state)
	{
		if (state.IsSyncing) return;
		if (dataGrid.SelectionMode == DataGridSelectionMode.Single) return;

		state.IsSyncing = true;
		try
		{
			selectedItems.Clear();
			foreach (var item in dataGrid.SelectedItems)
				selectedItems.Add(item);
		}
		finally
		{
			state.IsSyncing = false;
		}
	}

	/// <summary>
	/// Pushes bound collection changes back into the DataGrid selection.
	/// Handles the case where Initialize() populates the VM collection after data is loaded.
	/// </summary>
	private static void OnCollectionChanged(DataGrid dataGrid, ObservableCollection<object> selectedItems, SyncState state)
	{
		if (state.IsSyncing) return;
		if (dataGrid.SelectionMode == DataGridSelectionMode.Single) return;

		state.IsSyncing = true;
		try
		{
			dataGrid.SelectedItems.Clear();
			foreach (var item in selectedItems)
				dataGrid.SelectedItems.Add(item);
		}
		finally
		{
			state.IsSyncing = false;
		}
	}

	private sealed class SyncState
	{
		public bool IsSyncing;
		public NotifyCollectionChangedEventHandler? CollectionChangedHandler;
		public SelectionChangedEventHandler? DataGridSelectionChangedHandler;
	}
}
