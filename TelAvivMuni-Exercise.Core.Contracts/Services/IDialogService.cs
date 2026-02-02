namespace TelAvivMuni_Exercise.Core.Contracts;

/// <summary>
/// Service for displaying modal dialogs in a UI-agnostic way.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows a data browser dialog for selecting items from a collection.
    /// </summary>
    /// <typeparam name="T">The type of items to browse.</typeparam>
    /// <param name="items">The collection of items to display.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="selectedItem">The initially selected item.</param>
    /// <param name="columnConfiguration">Optional column configuration.</param>
    /// <returns>The selected item, or null if cancelled.</returns>
    T? ShowDataBrowserAsync<T>(
        IEnumerable<T> items,
        string title,
        T? selectedItem = null,
        IColumnConfiguration? columnConfiguration = null) where T : class;
}