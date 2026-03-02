using System.Collections.ObjectModel;

namespace TelAvivMuni_Exercise.Core.Contracts.ViewModels;

/// <summary>
/// Exposes the multi-selection state of a dialog ViewModel so that
/// UI code (e.g. DataBrowserDialog code-behind) can interact with it
/// without creating a circular project reference.
/// </summary>
public interface IMultiSelectViewModel
{
	/// <summary>Gets whether multiple selection is enabled.</summary>
	bool AllowMultipleSelection { get; }

	/// <summary>The currently selected items (populated by the DataGrid code-behind).</summary>
	ObservableCollection<object> SelectedItems { get; }
}
