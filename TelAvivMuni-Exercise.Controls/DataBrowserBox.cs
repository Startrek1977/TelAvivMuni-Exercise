using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using TelAvivMuni_Exercise.Core.Contracts;
using TelAvivMuni_Exercise.Domain;

namespace TelAvivMuni_Exercise.Controls;

/// <summary>
/// A custom WPF control that combines a read-only textbox with browse and clear buttons.
/// Displays the selected item and allows browsing through a collection via a dialog.
/// Supports both single-item and multi-item selection via <see cref="AllowMultipleSelection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class DataBrowserBox : Control, IColumnConfiguration
{
	/// <summary>
	/// Dependency property for the collection of items to browse.
	/// </summary>
	public static readonly DependencyProperty ItemsSourceProperty =
		DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(DataBrowserBox),
			new PropertyMetadata(null));

	/// <summary>
	/// Dependency property for the currently selected item (single-select mode).
	/// Supports two-way binding and triggers SelectionChanged event when modified.
	/// </summary>
	public static readonly DependencyProperty SelectedItemProperty =
		DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(DataBrowserBox),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

	/// <summary>
	/// Dependency property for the currently selected items (multi-select mode).
	/// Supports two-way binding and triggers display text update when modified.
	/// </summary>
	public static readonly DependencyProperty SelectedItemsProperty =
		DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(DataBrowserBox),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsChanged));

	/// <summary>
	/// Dependency property for whether multiple items can be selected at once.
	/// </summary>
	public static readonly DependencyProperty AllowMultipleSelectionProperty =
		DependencyProperty.Register(nameof(AllowMultipleSelection), typeof(bool), typeof(DataBrowserBox),
			new PropertyMetadata(false));

	/// <summary>
	/// Read-only dependency property key indicating whether any item is currently selected.
	/// Used by the control template to show/hide the clear button.
	/// </summary>
	private static readonly DependencyPropertyKey HasSelectionPropertyKey =
		DependencyProperty.RegisterReadOnly(nameof(HasSelection), typeof(bool), typeof(DataBrowserBox),
			new FrameworkPropertyMetadata(false));

	/// <summary>
	/// Read-only dependency property that is true when any item is selected (single or multi).
	/// </summary>
	public static readonly DependencyProperty HasSelectionProperty = HasSelectionPropertyKey.DependencyProperty;

	/// <summary>
	/// Dependency property for specifying which property of the selected item to display in the textbox.
	/// If not specified, ToString() is used.
	/// </summary>
	public static readonly DependencyProperty DisplayMemberPathProperty =
		DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string), typeof(DataBrowserBox),
			new PropertyMetadata(null));

	/// <summary>
	/// Dependency property for the title of the browse dialog window.
	/// </summary>
	public static readonly DependencyProperty DialogTitleProperty =
		DependencyProperty.Register(nameof(DialogTitle), typeof(string), typeof(DataBrowserBox),
			new PropertyMetadata(null));

	/// <summary>
	/// Dependency property for the dialog service used to show the browse dialog.
	/// </summary>
	public static readonly DependencyProperty DialogServiceProperty =
		DependencyProperty.Register(nameof(DialogService), typeof(IDialogService), typeof(DataBrowserBox),
			new PropertyMetadata(null));

	/// <summary>
	/// Dependency property for custom column configurations in the browse dialog's DataGrid.
	/// </summary>
	public static readonly DependencyProperty ColumnsProperty =
		DependencyProperty.Register(nameof(Columns), typeof(ObservableCollection<BrowserColumn>), typeof(DataBrowserBox),
			new PropertyMetadata(null));

	/// <summary>
	/// Routed event fired when the SelectedItem changes.
	/// </summary>
	public static readonly RoutedEvent SelectionChangedEvent =
		EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble,
			typeof(SelectionChangedEventHandler), typeof(DataBrowserBox));

	/// <summary>
	/// Event raised when the selected item changes.
	/// </summary>
	public event SelectionChangedEventHandler SelectionChanged
	{
		add => AddHandler(SelectionChangedEvent, value);
		remove => RemoveHandler(SelectionChangedEvent, value);
	}

	/// <summary>
	/// Property change callback for SelectedItem.
	/// Updates the display text and raises the SelectionChanged event.
	/// </summary>
	private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var control = (DataBrowserBox)d;
		control.UpdateHasSelection();
		control.UpdateDisplayText();

		// Raise the SelectionChanged event for consumers of this control
		var removedItems = e.OldValue != null ? [e.OldValue] : Array.Empty<object>();
		var addedItems = e.NewValue != null ? [e.NewValue] : Array.Empty<object>();
		control.RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, removedItems, addedItems));
	}

	/// <summary>
	/// Property change callback for SelectedItems.
	/// Updates the display text and subscribes to CollectionChanged so mutations
	/// (Add/Remove/Clear) also refresh the display.
	/// </summary>
	private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var control = (DataBrowserBox)d;

		// Unsubscribe from the old collection to avoid leaks / stale handlers
		if (e.OldValue is System.Collections.Specialized.INotifyCollectionChanged oldCollection)
			oldCollection.CollectionChanged -= control.OnSelectedItemsCollectionChanged;

		// Subscribe to the new collection so in-place mutations update the display
		if (e.NewValue is System.Collections.Specialized.INotifyCollectionChanged newCollection)
			newCollection.CollectionChanged += control.OnSelectedItemsCollectionChanged;

		control.UpdateHasSelection();
		control.UpdateDisplayText();
	}

	/// <summary>
	/// Handles in-place changes to the SelectedItems collection (Add / Remove / Clear).
	/// Keeps HasSelection and the display text in sync without requiring a full DP reassignment.
	/// </summary>
	private void OnSelectedItemsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
		UpdateHasSelection();
		UpdateDisplayText();
	}

	/// <summary>
	/// Gets or sets the collection of items available for browsing.
	/// </summary>
	public IEnumerable ItemsSource
	{
		get => (IEnumerable)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	/// <summary>
	/// Gets or sets the currently selected item from the collection (single-select mode).
	/// </summary>
	public object? SelectedItem
	{
		get => GetValue(SelectedItemProperty);
		set => SetValue(SelectedItemProperty, value);
	}

	/// <summary>
	/// Gets or sets the currently selected items (multi-select mode).
	/// </summary>
	public IList? SelectedItems
	{
		get => (IList?)GetValue(SelectedItemsProperty);
		set => SetValue(SelectedItemsProperty, value);
	}

	/// <summary>
	/// Gets whether multiple selection is enabled for this control.
	/// </summary>
	public bool AllowMultipleSelection
	{
		get => (bool)GetValue(AllowMultipleSelectionProperty);
		set => SetValue(AllowMultipleSelectionProperty, value);
	}

	/// <summary>
	/// Gets whether any item is currently selected (single or multi).
	/// </summary>
	public bool HasSelection
	{
		get => (bool)GetValue(HasSelectionProperty);
		private set => SetValue(HasSelectionPropertyKey, value);
	}

	/// <summary>
	/// Gets or sets the property path to use for displaying the selected item.
	/// </summary>
	public string DisplayMemberPath
	{
		get => (string)GetValue(DisplayMemberPathProperty);
		set => SetValue(DisplayMemberPathProperty, value);
	}

	/// <summary>
	/// Gets or sets the title for the browse dialog.
	/// </summary>
	public string DialogTitle
	{
		get => (string)GetValue(DialogTitleProperty);
		set => SetValue(DialogTitleProperty, value);
	}

	/// <summary>
	/// Gets or sets the dialog service for showing the browse dialog.
	/// </summary>
	public IDialogService DialogService
	{
		get => (IDialogService)GetValue(DialogServiceProperty);
		set => SetValue(DialogServiceProperty, value);
	}

	/// <summary>
	/// Gets or sets the custom column configurations for the browse dialog's DataGrid.
	/// </summary>
	public ObservableCollection<BrowserColumn> Columns
	{
		get => (ObservableCollection<BrowserColumn>)GetValue(ColumnsProperty);
		set => SetValue(ColumnsProperty, value);
	}

	public bool HasCustomColumns => Columns != null && Columns.Count > 0;

	IEnumerable<BrowserColumn>? IColumnConfiguration.Columns => Columns;

	// Template part references
	private Button? _browseButton;
	private TextBox? _textBox;
	private Button? _clearButton;

	/// <summary>
	/// Initializes a new instance of the DataBrowserBox control.
	/// </summary>
	public DataBrowserBox()
    {
        Columns = [];
	}

	/// <summary>
	/// Static constructor to override default style key.
	/// This ensures the control uses the style defined in Generic.xaml.
	/// </summary>
	static DataBrowserBox()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(DataBrowserBox),
			new FrameworkPropertyMetadata(typeof(DataBrowserBox)));
	}

	/// <summary>
	/// Called when the control template is applied.
	/// Retrieves template parts and subscribes to their events.
	/// </summary>
	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		// Unsubscribe from previous template parts to prevent memory leaks
		if (_browseButton != null)
		{
			_browseButton.Click -= OnBrowseButtonClick;
		}

		if (_clearButton != null)
		{
			_clearButton.Click -= OnClearButtonClick;
		}

		// Retrieve new template parts from the control template
		_browseButton = GetTemplateChild("PART_BrowseButton") as Button;
		_textBox = GetTemplateChild("PART_TextBox") as TextBox;
		_clearButton = GetTemplateChild("PART_ClearButton") as Button;

		// Subscribe to template part events
		if (_browseButton != null)
		{
			_browseButton.Click += OnBrowseButtonClick;
		}

		if (_clearButton != null)
		{
			_clearButton.Click += OnClearButtonClick;
		}

		// Update the display to reflect the current SelectedItem
		UpdateHasSelection();
		UpdateDisplayText();
	}

	/// <summary>
	/// Handles the Browse button click event.
	/// Opens a dialog to allow the user to select an item from the collection.
	/// </summary>
	private void OnBrowseButtonClick(object sender, RoutedEventArgs e)
	{
		// Validate that a dialog service is configured
		if (DialogService == null)
			return;

		// Validate that items are available to browse
		if (ItemsSource == null)
		{
			MessageBox.Show("No items available to browse.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}

		var title = DialogTitle ?? "Select Item";
		var allItems = ItemsSource.Cast<object>().ToList();

		if (AllowMultipleSelection)
		{
			// Multi-select: pass current selection as a list and receive a list back
			var currentSelection = SelectedItems?.Cast<object>().ToList()
				?? (IReadOnlyList<object>)[];

			var result = DialogService.ShowDataBrowserMultiSelect(
				allItems, title, currentSelection, this);

			// Update SelectedItems with the returned list
			if (SelectedItems != null)
			{
				SelectedItems.Clear();
				foreach (var item in result)
					SelectedItems.Add(item);
			}
			else
			{
				// Create a new list if none exists
				var newList = new System.Collections.ObjectModel.ObservableCollection<object>(result);
				SelectedItems = newList;
			}

			// Explicitly sync HasSelection and display text after any in-place mutation.
			// In-place Clear/Add does not change the SelectedItems DP reference, so
			// OnSelectedItemsChanged is never re-invoked; the CollectionChanged subscription
			// may not fire synchronously in all scenarios.  Mirror the explicit-call pattern
			// used by OnClearButtonClick to guarantee a reliable update.
			UpdateHasSelection();
			UpdateDisplayText();
		}
		else
		{
			// Single-select: existing behaviour
			var result = DialogService.ShowDataBrowserAsync(allItems, title, SelectedItem, this);

			if (result != null)
			{
				SelectedItem = result;
			}
		}
	}

	/// <summary>
	/// Handles the Clear button click event.
	/// Clears the currently selected item(s).
	/// </summary>
	private void OnClearButtonClick(object sender, RoutedEventArgs e)
	{
		if (AllowMultipleSelection)
		{
			SelectedItems?.Clear();
			UpdateHasSelection();
			UpdateDisplayText();
		}
		else
		{
			SelectedItem = null;
		}
	}

	/// <summary>
	/// Updates the HasSelection property based on the current selection state.
	/// Also directly sets the clear button's visibility as a safety net, so the
	/// button responds even in scenarios where the ControlTemplate.Trigger does
	/// not re-evaluate (e.g. in-place collection mutations that do not change the
	/// DP reference and therefore do not always trigger a property-engine notification).
	/// </summary>
	private void UpdateHasSelection()
	{
		HasSelection = AllowMultipleSelection
			? SelectedItems != null && SelectedItems.Count > 0
			: SelectedItem != null;

		if (_clearButton != null)
			_clearButton.Visibility = HasSelection ? Visibility.Visible : Visibility.Collapsed;
	}

	/// <summary>
	/// Updates the textbox display text based on the current selection.
	/// Single-select: shows item display value or placeholder.
	/// Multi-select (1 item): shows item display value.
	/// Multi-select (2+ items): shows "&lt;first item&gt; (+N products)".
	/// </summary>
	private void UpdateDisplayText()
	{
		if (_textBox == null) return;

		if (AllowMultipleSelection)
		{
			var count = SelectedItems?.Count ?? 0;
			if (count == 0)
			{
				_textBox.Text = "Click to select...";
				_textBox.Opacity = 0.5;
			}
			else if (count == 1)
			{
				_textBox.Text = GetDisplayValue(SelectedItems![0]!);
				_textBox.Opacity = 1.0;
			}
			else
			{
				var firstName = GetDisplayValue(SelectedItems![0]!);
				_textBox.Text = $"{firstName} (+{count - 1} products)";
				_textBox.Opacity = 1.0;
			}
		}
		else
		{
			if (SelectedItem == null)
			{
				// Show placeholder text with reduced opacity when no item is selected
				_textBox.Text = "Click to select...";
				_textBox.Opacity = 0.5;
			}
			else
			{
				// Show the selected item's display value with full opacity
				_textBox.Text = GetDisplayValue(SelectedItem);
				_textBox.Opacity = 1.0;
			}
		}
	}

	/// <summary>
	/// Retrieves the display value for the given item.
	/// Uses DisplayMemberPath if specified, otherwise uses ToString().
	/// </summary>
	/// <param name="item">The item to get the display value for</param>
	/// <returns>The display string for the item</returns>
	private string GetDisplayValue(object item)
	{
		// If no DisplayMemberPath is specified, use ToString()
		if (string.IsNullOrEmpty(DisplayMemberPath))
		{
			return item.ToString() ?? string.Empty;
		}

		// Use reflection to get the specified property value
		var property = item.GetType().GetProperty(DisplayMemberPath);
		if (property != null)
		{
			var value = property.GetValue(item);
			return value?.ToString() ?? string.Empty;
		}

		// Fallback to ToString() if the property is not found
		return item.ToString() ?? string.Empty;
	}
}
